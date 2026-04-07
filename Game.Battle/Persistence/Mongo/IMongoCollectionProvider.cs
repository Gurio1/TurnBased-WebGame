using MongoDB.Driver;

namespace Game.Battle.Persistence.Mongo;

public interface IMongoCollectionProvider
{
    IMongoCollection<TDocument> GetCollection<TDocument>();
}
