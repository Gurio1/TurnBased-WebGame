using Game.Core.Abilities;
using Game.Core.Models;
using Game.Data.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Game.Features.Players;

public class PlayersService
{
    private readonly IOptions<MongoSettings> _mongoDatabaseSettings;
    private readonly IMongoCollection<Hero> _playersCollection;
    private readonly IMongoDatabase _mongoDatabase;

    //TODO: Implement error handling(or use EF Core)
    public PlayersService(
        IOptions<MongoSettings> mongoDatabaseSettings)
    {
        _mongoDatabaseSettings = mongoDatabaseSettings;
        var mongoClient = new MongoClient(
            mongoDatabaseSettings.Value.ConnectionString);

        _mongoDatabase = mongoClient.GetDatabase(
            mongoDatabaseSettings.Value.DatabaseName);

        _playersCollection = _mongoDatabase.GetCollection<Hero>(
            mongoDatabaseSettings.Value.PlayersCollectionName);
        
    }


    public async Task<Hero> GetById(string playerId)
    {
        try
        {
            return await _playersCollection.Find(a => a.Id == playerId).FirstOrDefaultAsync();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
    
    public async Task<Hero> GetByIdWithAbilities(string playerId)
    {
        try
        {
            var lookupResult =  await _playersCollection.AsQueryable()
                .Where(p => p.Id == playerId)
                .Lookup(_mongoDatabase.GetCollection<Ability>(_mongoDatabaseSettings.Value.AbilitiesCollectionName),
                    (pl, ab) => ab
                        .Where(ability => pl.AbilityIds.Contains(ability.Id))).FirstOrDefaultAsync();

            lookupResult.Local.Abilities = lookupResult.Results.ToList();

            return lookupResult.Local;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task CreateAsync(Hero newPlayer)
    {
        try
        {
            await _playersCollection.InsertOneAsync(newPlayer);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task UpdateAsync(Hero updatedPlayer)
    {
        var test = updatedPlayer.ToBsonDocument();
        await _playersCollection.ReplaceOneAsync(x => x.Id == updatedPlayer.Id, updatedPlayer);
    }
        

    public async Task RemoveAsync(string id) =>
        await _playersCollection.DeleteOneAsync(x => x.Id == id);
}