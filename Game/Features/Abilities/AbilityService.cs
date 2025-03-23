using Game.Core.Abilities;
using Game.Data.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Game.Features.Abilities;

public class AbilityService : IAbilityService
{
    private readonly IMongoCollection<Ability> _abilitiesCollection;

    //TODO: Implement error handling(or use EF Core)
    public AbilityService(
        IOptions<MongoSettings> mongoDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            mongoDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            mongoDatabaseSettings.Value.DatabaseName);

        _abilitiesCollection = mongoDatabase.GetCollection<Ability>(
            mongoDatabaseSettings.Value.AbilitiesCollectionName);
        
    }
    public async Task<Ability> GetById(string id)
    {
        return await _abilitiesCollection.Find(a => a.Id == id).FirstOrDefaultAsync();
    }
    
    public async Task<List<Ability>> GetByIdsAsync(List<string> ids)
    {
        Console.WriteLine($"Searching for IDs: {string.Join(", ", ids)}");

        var filter = Builders<Ability>.Filter.In(a => a.Id, ids);
        var abilities = await _abilitiesCollection.Find(filter).ToListAsync();

        Console.WriteLine($"Found {abilities.Count} abilities");
        return abilities;
    }
    
    public async Task Save(Ability ability)
    {
        await _abilitiesCollection.InsertOneAsync(ability);
    }
}