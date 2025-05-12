using Game.Features.Identity;
using Game.Persistence.Mongo;
using Game.Persistence.Queries;
using Game.Persistence.Redis;
using Game.Persistence.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Game.Persistence;

public static class PersistenceExtension
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
        
        services.AddTransient<IMongoCollectionProvider, MongoCollectionProvider>();
        
        services.AddScoped<IPlayerRepository, PlayerMongoRepository>();
        //TODO: Should it be a repo?
        services.AddScoped<GetMonsterQuery>();
        
        services.AddSingleton<RedisProvider>();
        
        return services;
    }
}
