using System.Text;
using Game.Battle.Application.Battle;
using Game.Battle.Application.Battle.EventHandlers;
using Game.Battle.Core.Battle;
using Game.Battle.Core.Battle.PVE;
using Game.Battle.Core.Models;
using Game.Battle.Messaging.Clients;
using Game.Battle.Persistence.Cache.Redis;
using Game.Battle.Persistence.Mongo;
using Game.Battle.SignalR;
using Game.SharedKernel.Battle;
using Game.SharedKernel.Contracts.Requests;
using Game.SharedKernel.Messaging;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MongoDB.Driver;

var builder = WebApplication.CreateBuilder(args);
var rabbitMqSettings = builder.Configuration.GetSection("Messaging:RabbitMq").Get<RabbitMqSettings>() ?? new RabbitMqSettings();

builder.Host.UseDefaultServiceProvider(opt =>
{
    opt.ValidateScopes = true;
    opt.ValidateOnBuild = true;
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Game.Battle API",
        Version = "v1"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
builder.Services.AddSignalR();
builder.Services.AddHttpContextAccessor();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSecret = builder.Configuration["Auth:JwtSecret"];
        if (string.IsNullOrWhiteSpace(jwtSecret))
            throw new InvalidOperationException("Missing configuration value: Auth:JwtSecret.");

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("ClientApp", policy =>
    {
        policy.WithOrigins("https://localhost:4200", "http://localhost:4200")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});

builder.Services.AddSingleton<RedisProvider>();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    options.InstanceName = "BattlePlayerRelations:";
});

builder.Services.AddSingleton<IMongoClient>(_ =>
{
    string? connectionString = builder.Configuration.GetConnectionString("MongoConnection");
    return new MongoClient(connectionString);
});

builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("Messaging:RabbitMq"));
builder.Services.AddMassTransit(x =>
{
    x.AddRequestClient<BattleResolveRequest>(new Uri($"queue:{rabbitMqSettings.BattleSettlementRequestQueue}"));
    x.AddRequestClient<BattleStartSnapshotRequest>(new Uri($"queue:{rabbitMqSettings.BattleStartSnapshotRequestQueue}"));

    x.UsingRabbitMq((_, cfg) =>
    {
        cfg.Host(rabbitMqSettings.HostName, rabbitMqSettings.VirtualHost, h =>
        {
            h.Username(rabbitMqSettings.UserName);
            h.Password(rabbitMqSettings.Password);
        });
    });
});
builder.Services.AddScoped<IBattleSettlementClient, BattleSettlementClient>();
builder.Services.AddScoped<IGameBattleSnapshotClient, GameBattleSnapshotClient>();

builder.Services.AddSingleton<IOptions<MongoSettings>>(_ =>
    Options.Create(new MongoSettings
    {
        DatabaseName = builder.Configuration["MongoDatabase:DatabaseName"] ?? "GameBattle",
        CollectionNames = new Dictionary<string, string>
        {
            [nameof(Player)] = "Players",
            [nameof(Monster)] = "Monsters",
            [nameof(PveBattle)] = "Battles"
        }
    }));

builder.Services.AddSingleton<IMongoCollectionProvider, MongoCollectionProvider>();
builder.Services.AddScoped<IBattleReadRepository, BattleMongoRepository>();
builder.Services.AddScoped<IBattleRepository, BattleRedisRepository>();
builder.Services.AddScoped<BattleContext>();
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
builder.Services.AddScoped<IBattleUserContext, BattleUserContext>();
builder.Services.AddScoped<PlayerBattleCache>();
builder.Services.AddScoped<PveBattleVictoryHandler>();
builder.Services.AddScoped<PveBattleDefeatHandler>();
builder.Services.AddScoped<IPveBattleDomainEventProcessor, PveBattleDomainEventProcessor>();
builder.Services.AddScoped<IPveBattleService, PveBattleService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("ClientApp");
app.UseMiddleware<WebSocketsMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<PveBattleHub>("/hubs/battle");

app.Run();
