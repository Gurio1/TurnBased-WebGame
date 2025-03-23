using Game.Identity;
using Microsoft.AspNetCore.Identity;

namespace Game.Features.Identity;

public static class IdentityExtension
{
    public static IServiceCollection AddIdentityServices(
        this IServiceCollection services,
        ConfigurationManager config)
    {
        services.Configure<IdentityOptions>(options =>
        {
            options.Password.RequireUppercase = false;
            options.Password.RequireLowercase = false;
            options.User.RequireUniqueEmail = true;
        });
        
        services.AddScoped<ITokenFactory, TokenFactory>();
        

        return services;
    }
}