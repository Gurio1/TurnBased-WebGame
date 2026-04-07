using Game.Application.Equipment.Blueprints;
using Game.Application.Locations;
using Game.Application.Monsters;
using Game.Application.Players;
using Game.Core.Equipment;
using Game.Core.Location;
using Game.Core.Models;
using Game.Core.PlayerProfile;
using Game.Persistence.Mongo;
using Game.Persistence.Repositories;

namespace Game.Persistence;

public static class PersistenceExtension
{
    public static IServiceCollection AddDataServices(
        this IServiceCollection services)
    {
        services.AddTransient<IMongoCollectionProvider, MongoCollectionProvider>();
        
        services.AddScoped<IPlayerRepository, PlayerMongoRepository>();
        services.AddScoped<IMonsterRepository, MonsterMongoRepository>();
        services.AddScoped<IEquipmentBlueprintRepository, EquipmentBlueprintMongoRepository>();
        services.AddSingleton<ILocationRepository, PredefinedLocationRepository>();
        services.AddScoped<IPlayerService, PlayerService>();
        services.AddScoped<IMonsterService, MonsterService>();
        services.AddScoped<IEquipmentBlueprintService, EquipmentBlueprintService>();
        services.AddScoped<ILocationService, LocationService>();
        
        return services;
    }
}

