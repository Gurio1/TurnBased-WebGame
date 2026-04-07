using System.Text;
using System.Text.Json;
using Game.Identity.Persistence;
using Game.Identity.Services;
using Game.SharedKernel.Contracts.Requests;
using Game.SharedKernel.Messaging;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

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
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Game.Identity API",
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
        string jwtSecret = builder.Configuration["Auth:JwtSecret"]!;
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

builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
    options.User.RequireUniqueEmail = true;
});

builder.Services.AddScoped<ITokenFactory, TokenFactory>();
builder.Services.Configure<RabbitMqSettings>(builder.Configuration.GetSection("Messaging:RabbitMq"));
builder.Services.AddMassTransit(x =>
{
    x.AddRequestClient<CreatePlayerRequest>(new Uri($"queue:{rabbitMqSettings.PlayerCreateRequestQueue}"));
    x.AddRequestClient<DeletePlayerRequest>(new Uri($"queue:{rabbitMqSettings.PlayerDeleteRequestQueue}"));

    x.UsingRabbitMq((_, cfg) =>
    {
        cfg.Host(rabbitMqSettings.HostName, rabbitMqSettings.VirtualHost, h =>
        {
            h.Username(rabbitMqSettings.UserName);
            h.Password(rabbitMqSettings.Password);
        });
    });
});
builder.Services.AddScoped<IGamePlayerProvisioningClient, GamePlayerProvisioningClient>();

builder.Services.AddPersistenceServices(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

app.UseAuthentication()
    .UseAuthorization();

app.UseHttpsRedirection();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapControllers();

app.Run();
