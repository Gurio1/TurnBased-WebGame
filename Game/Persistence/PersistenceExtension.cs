using Game.Core.Battle;
using Game.Core.PlayerProfile;
using Game.Features.Identity;
using Game.Persistence.Mongo;
using Game.Persistence.Redis;
using Game.Persistence.Repositories;
using Game.Persistence.Requests;
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
        services.AddScoped<IBattleRepository, BattleRedisRepository>();
        
        //TODO: Should it be in a repo?
        services.AddScoped<GetMonsterQuery>();
        services.AddScoped<UpdatePlayerAfterEquipmentInteraction>();
        services.AddScoped<UpdatePlayerAfterSellInteraction>();
        
        services.AddSingleton<RedisProvider>();
        
        return services;
    }
}
