using MongoDB.Driver;

namespace Game.Persistence.Mongo;

public interface IMongoCollectionProvider
{
    IMongoCollection<TDocument> GetCollection<TDocument>();
}
