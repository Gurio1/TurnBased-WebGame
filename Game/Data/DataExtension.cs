using Game.Data.Mongo;
using Game.Features.Identity;
using Microsoft.EntityFrameworkCore;

namespace Game.Data;

public static class DataExtension
{
    public static IServiceCollection AddDataServices(
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
        
        services.AddTransient(typeof(IMongoCollectionProvider<>),
            typeof(MongoCollectionProvider<>));
        
        return services;
    }
}
