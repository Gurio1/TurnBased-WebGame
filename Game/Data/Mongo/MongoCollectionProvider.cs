using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Game.Data.Mongo;

public class MongoCollectionProvider<TDocument> : IMongoCollectionProvider<TDocument>
{
    public MongoCollectionProvider(
        IMongoClient client,
        IOptions<MongoSettings> opts)
    {
        var settings = opts.Value;
        var db = client.GetDatabase(settings.DatabaseName);
        
        string typeName = typeof(TDocument).Name;
        
        if (!settings.CollectionNames.TryGetValue(typeName, out string? collName))
            throw new InvalidOperationException(
                $"No mongo collection name configured for document type '{typeName}'");
        
        Collection = db.GetCollection<TDocument>(collName);
    }
    
    public IMongoCollection<TDocument> Collection { get; }
}
