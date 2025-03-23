using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;

namespace Game.Features.Players;

public class PlayerQuery
{
    private readonly List<BsonDocument> _pipeline = new();
    private readonly IMongoCollection<BsonDocument> _playersCollection;

    public PlayerQuery(IMongoCollection<BsonDocument> playersCollection)
    {
        _playersCollection = playersCollection;
    }
    
    public PlayerQuery GetById(string playerId)
    {
        _pipeline.Add(new BsonDocument("$match", new BsonDocument("_id", playerId)));
        return this;
    }
    
    /*public PlayerQuery WithEquipment()
    {
        _pipeline.Add(new BsonDocument("$lookup", new BsonDocument
        {
            { "from", "equipment" },
            { "localField", "EquipmentIds" },
            { "foreignField", "_id" },
            { "as", "Equipment" }
        }));
        
        _pipeline.Add(new BsonDocument("$unset", "EquipmentIds"));
        
        return this;
    }*/
    
    public PlayerQuery WithAbilities()
    {
        _pipeline.Add(new BsonDocument("$lookup", new BsonDocument
        {
            { "from", "abilities" },
            { "localField", "AbilityIds" },
            { "foreignField", "_id" },
            { "as", "Abilities" }
        }));
        _pipeline.Add(new BsonDocument("$unset", "AbilityIds"));
        return this;
    }

    public async Task<T> ExecuteAsync<T>()
    {
        var bson = await _playersCollection.Aggregate<BsonDocument>(_pipeline).FirstAsync();

        return BsonSerializer.Deserialize<T>(bson);

    }


    
    
}