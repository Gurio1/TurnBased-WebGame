using Game.Features.Identity;
using Microsoft.EntityFrameworkCore;

namespace Game.Data;

public static class DataExtension
{
    public static IServiceCollection AddDataServices(
        this IServiceCollection services,
        ConfigurationManager config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<ApplicationDbContext>(builder =>
        {
            builder.UseNpgsql(connectionString);
        });

        services.AddIdentityCore<User>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        
        return services;
    }
}