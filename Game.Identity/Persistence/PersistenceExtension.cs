using Game.Identity.Core;
using Microsoft.EntityFrameworkCore;

namespace Game.Identity.Persistence;

public static class PersistenceExtension
{
    public static IServiceCollection AddPersistenceServices(
        this IServiceCollection services,
        ConfigurationManager config)
    {
        string? connectionString = config.GetConnectionString("DefaultConnection");
        
        services.AddDbContext<ApplicationDbContext>(builder =>
        {
            builder.UseNpgsql(connectionString);
        });
        
        services.AddIdentityCore<User>()
            .AddEntityFrameworkStores<ApplicationDbContext>();
        
        return services;
    }
}
