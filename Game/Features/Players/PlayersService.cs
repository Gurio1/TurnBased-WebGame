using Game.Core.Models;
using Game.Data.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;

namespace Game.Features.Players;

public class PlayersService
{
    private readonly IMongoCollection<BsonDocument> _playersCollection;

    //TODO: Implement error handling(or use EF Core)
    public PlayersService(
        IOptions<MongoSettings> mongoDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            mongoDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            mongoDatabaseSettings.Value.DatabaseName);

        _playersCollection = mongoDatabase.GetCollection<BsonDocument>(
            mongoDatabaseSettings.Value.PlayersCollectionName);
        
    }

    public PlayerQuery CreateQuery() =>
        new(_playersCollection);
    

    public async Task CreateAsync(Hero newPlayer)
    {
        try
        {
            await _playersCollection.InsertOneAsync(newPlayer.ToBsonDocument());
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public async Task UpdateAsync(string id, Hero updatedPlayer)
    {
        var test = updatedPlayer.ToBsonDocument();
        await _playersCollection.ReplaceOneAsync(x => x["_id"] == id, test);
    }
        

    public async Task RemoveAsync(string id) =>
        await _playersCollection.DeleteOneAsync(x => x["_id"] == id);
}