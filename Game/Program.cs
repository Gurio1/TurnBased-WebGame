using System.Text.Json;
using System.Text.Json.Serialization;
using Game.Application;
using Game.Application.Battle;
using Game.Core.Battle;
using Game.Core.Equipment.Generation;
using Game.Core.Loot;
using Game.Features.Battle.PVE;
using Game.Features.Equipment.Generation;
using Game.Persistence;
using Game.Persistence.Mongo;
using Game.SharedKernel;
using Game.SignalR;
using Game.Utilities;
using Game.Utilities.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

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
builder.Services.AddSingleton<ILootService, LootService>();
builder.Services.AddSingleton<IEquipmentGenerator, EquipmentGenerator>();

builder.Services.AddScoped<IBattleAuthService, BattleAuthService>();
builder.Services.AddScoped<BattleContext>();
builder.Services.AddScoped<BattleCacheManager>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<UrlBuilder>();


builder.Services.AddDataServices();
builder.Services.RegisterDispatcher(typeof(Program));

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

app.UseCors("AllowSpecificOrigin");

using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.InitializeAbilities();
}

app.UseMiddleware<WebSocketsMiddleware>();

app.UseAuthentication()
    .UseAuthorization();

app.MapControllers();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();
app.UseMiddleware<ExecutionTimeMiddleware>();

app.MapHub<PveBattleHub>("/hubs/battle");

app.Run();
