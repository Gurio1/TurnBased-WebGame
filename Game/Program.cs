using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Game.Application.Equipment.Generation;
using Game.Core.Equipment.Generation;
using Game.Core.Loot;
using Game.Messaging;
using Game.Persistence;
using Game.Persistence.Mongo;
using Game.SharedKernel.Messaging;
using Game.SignalR;
using Game.Utilities;
using Game.Utilities.Middlewares;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
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

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Game API",
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

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var jwtSecret = builder.Configuration["Auth:JwtSecret"]!;
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

//For SignalR
builder.Services.AddSignalR()
    .AddJsonProtocol(options =>
    {
        options.PayloadSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.Configure<MongoSettings>(
    builder.Configuration.GetSection("MongoDatabase"));

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("RedisConnection");
    options.InstanceName = "BattlePlayerRelations:";
});

MongoDbConfig.RegisterDiscriminator();

builder.Services.AddSingleton<IMongoClient>(sp =>
{
    string? connectionString = builder.Configuration.GetConnectionString("MongoConnection");
    return new MongoClient(connectionString);
});

builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
builder.Services.AddScoped<ILootService, LootService>();
builder.Services.AddScoped<IEquipmentGenerator, EquipmentGenerator>();
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("Messaging:RabbitMq"));
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<BattleSettlementConsumer>();
    x.AddConsumer<CreatePlayerConsumer>();
    x.AddConsumer<DeletePlayerConsumer>();
    x.AddConsumer<BattleSnapshotConsumer>();

    x.UsingRabbitMq((context, cfg) =>
    {
        cfg.Host(rabbitMqSettings.HostName, rabbitMqSettings.VirtualHost, h =>
        {
            h.Username(rabbitMqSettings.UserName);
            h.Password(rabbitMqSettings.Password);
        });

        cfg.ReceiveEndpoint(rabbitMqSettings.BattleSettlementRequestQueue, e =>
        {
            e.ConfigureConsumer<BattleSettlementConsumer>(context);
        });

        cfg.ReceiveEndpoint(rabbitMqSettings.PlayerCreateRequestQueue, e =>
        {
            e.ConfigureConsumer<CreatePlayerConsumer>(context);
        });

        cfg.ReceiveEndpoint(rabbitMqSettings.PlayerDeleteRequestQueue, e =>
        {
            e.ConfigureConsumer<DeletePlayerConsumer>(context);
        });

        cfg.ReceiveEndpoint(rabbitMqSettings.BattleStartSnapshotRequestQueue, e =>
        {
            e.ConfigureConsumer<BattleSnapshotConsumer>(context);
        });
    });
});

builder.Services.AddScoped<UrlBuilder>();

builder.Services.AddDataServices();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policyBuilder =>
        {
            policyBuilder.WithOrigins("https://localhost:4200", "http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

app.UseHttpsRedirection();
app.UseMiddleware<WebSocketsMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowSpecificOrigin");
app.UseMiddleware<ExecutionTimeMiddleware>();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
