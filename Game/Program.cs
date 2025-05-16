using System.Text.Json.Serialization;
using DotNetEnv;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Game.Application;
using Game.Core.Equipment.Generation;
using Game.Core.Loot;
using Game.Core.SharedKernel;
using Game.Features.Battle;
using Game.Features.Battle.Models;
using Game.Features.Battle.PVE;
using Game.Features.Equipment.Generation;
using Game.Features.Identity;
using Game.Features.Identity.SignalR;
using Game.Persistence;
using Game.Persistence.Mongo;
using Game.Utilities.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;

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

app.UseFastEndpoints()
    .UseSwaggerGen();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
    app.MapOpenApi();

app.UseHttpsRedirection();
app.UseMiddleware<ExecutionTimeMiddleware>();
Env.Load();

app.MapHub<PveBattleHub>("/hubs/battle");

app.Run();
