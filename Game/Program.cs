using System.Text.Json;
using System.Text.Json.Serialization;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Game;
using Game.Core.Common;
using Game.Data;
using Game.Data.Mongo;
using Game.Features;
using Game.Features.Abilities;
using Game.Features.Battle;
using Game.Features.Battle.Hubs;
using Game.Features.Battle.Models;
using Game.Features.Drop;
using Game.Features.Equipment;
using Game.Features.EquipmentBlueprints;
using Game.Features.Identity;
using Game.Features.Identity.SignalR;
using Game.Features.Monsters;
using Game.Features.Players;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;
using SixLabors.ImageSharp;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider(opt =>
{
    opt.ValidateScopes = true;
    opt.ValidateOnBuild = true;
});

builder.Services.ConfigureHttpJsonOptions(opt =>
{
    opt.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

builder.Services
    .AddAuthenticationJwtBearer(options =>
    {
        options.SigningKey = builder.Configuration["Auth:JwtSecret"]!;
    })
    .AddAuthorization()
    .AddFastEndpoints()
    .SwaggerDocument();


//For SignalR
builder.Services.AddAuthentication(o => o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme);
builder.Services.AddAuthorization();

builder.Services.AddSignalR()
    .AddJsonProtocol(options => {
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
    string? connectionString =   builder.Configuration.GetConnectionString("MongoConnection");
    return new MongoClient(connectionString);
});
builder.Services.AddSingleton<IUserIdProvider, CustomUserIdProvider>();
builder.Services.AddSingleton<RedisProvider>();
builder.Services.AddSingleton<IPlayersMongoRepository,PlayersMongoRepository>();
builder.Services.AddSingleton<IMonstersMongoRepository,MonstersMongoRepository>();
builder.Services.AddSingleton<IEquipmentTemplateMongoRepository,EquipmentTemplateMongoRepository>();
builder.Services.AddSingleton<IAbilityMongoRepository, AbilityMongoRepository>();
builder.Services.AddSingleton<IBattleRepository, BattleRedisRepository>();
builder.Services.AddSingleton<IBattleService, BattleService>();
builder.Services.AddSingleton<IDropService,DropService>();
builder.Services.AddSingleton<EquipmentGenerator>();

builder.Services.AddScoped<PveBattleManager>();
builder.Services.AddScoped<BattleContext>();


builder.Services.AddDataServices(builder.Configuration);
builder.Services.AddIdentityServices();
builder.Services.RegisterDispatcher();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        policyBuilder =>
        {
            policyBuilder.WithOrigins("https://localhost:4200","http://localhost:4200")
                .AllowAnyHeader()
                .AllowAnyMethod();
        });
});

var app = builder.Build();

app.UseCors("AllowSpecificOrigin");

app.UseMiddleware<WebSocketsMiddleware>();

app.UseAuthentication()
    .UseAuthorization();

app.UseFastEndpoints()
    .UseSwaggerGen();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseMiddleware<ExecutionTimeMiddleware>();
DotNetEnv.Env.Load();

app.MapHub<BattleHub>("/hubs/battle");

app.Run();
