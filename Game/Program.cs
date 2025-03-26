using System.Text.Json.Serialization;
using FastEndpoints;
using FastEndpoints.Security;
using FastEndpoints.Swagger;
using Game.Data;
using Game.Data.Mongo;
using Game.Features.Abilities;
using Game.Features.Battle;
using Game.Features.Battle.Hubs;
using Game.Features.Drop;
using Game.Features.Equipment;
using Game.Features.Identity;
using Game.Features.Players;
using Microsoft.AspNetCore.Authentication.JwtBearer;

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

MongoDbConfig.RegisterDiscriminator();

builder.Services.AddScoped<DropService>();
builder.Services.AddScoped<PlayersService>();
builder.Services.AddScoped<BattleManager>();
builder.Services.AddScoped<RedisConnectionFactory>();
builder.Services.AddScoped<EquipmentTemplateService>();

builder.Services.AddScoped<IAbilityService, AbilityService>();
builder.Services.AddScoped<PlayersService>();
builder.Services.AddScoped<BattleService>();
builder.Services.AddScoped<EquipmentGenerator>();

builder.Services.AddDataServices(builder.Configuration);
builder.Services.AddIdentityServices(builder.Configuration);


builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));

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

app.MapHub<BattleHub>("/hubs/battle");

app.Run();