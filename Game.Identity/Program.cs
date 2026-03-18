using Game.Identity.Persistence;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Text;
using Game.Identity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;


var builder = WebApplication.CreateBuilder(args);

builder.Host.UseDefaultServiceProvider(opt =>
{
    opt.ValidateScopes = true;
    opt.ValidateOnBuild = true;
});

builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
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

builder.Services.AddHttpClient("GameApi", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["Services:GameApi:BaseUrl"]!);
});

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

app.MapControllers();

app.Run();
