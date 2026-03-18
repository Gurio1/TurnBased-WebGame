using Game.Core.Battle;
using Game.Core.PlayerProfile;
using Game.Features.Players.Sell;
using Game.Persistence.Mongo;
using Game.Persistence.Redis;
using Game.Persistence.Repositories;
using Game.Persistence.Requests;

namespace Game.Persistence;

public static class PersistenceExtension
{
    public static IServiceCollection AddDataServices(
        this IServiceCollection services)
    {
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
