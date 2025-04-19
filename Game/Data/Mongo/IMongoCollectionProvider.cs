using MongoDB.Driver;

namespace Game.Data.Mongo;

public interface IMongoCollectionProvider<TDocument>
{
    IMongoCollection<TDocument> Collection { get; }
}
