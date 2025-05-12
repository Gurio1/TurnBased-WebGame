using Game.Core.Abilities;
using Game.Persistence.Mongo;
using MongoDB.Driver;

namespace Game.Application;

public static class AbilityInitializer
{
    public static async Task InitializeAbilities(this IServiceProvider serviceProvider)
    {
        var collection = serviceProvider.GetService<IMongoCollectionProvider>()!.GetCollection<Ability>();
        
        var abilityTypes = AbilityTypes.Discover().ToArray();
        
        var filter = Builders<Ability>.Filter.In(a => a.TypeName, abilityTypes.Select(t => t.Name));
        
        var existingAbilityTypeNames = await collection.Find(filter)
            .Project(a => a.TypeName)
            .ToListAsync();
        
        var missingAbilities = abilityTypes
            .Where(t => !existingAbilityTypeNames.Contains(t.Name))
            .ToList();
        
        if (missingAbilities.Count > 0)
        {
            var newAbilities = missingAbilities.Select(AbilityActivator.CreateAbility);
            
            await collection.InsertManyAsync(newAbilities);
        }
    }
}
