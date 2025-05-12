using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Game.Persistence.Mongo;

public class MongoCollectionProvider : IMongoCollectionProvider
{
    private readonly MongoSettings settings;
    private readonly IMongoDatabase db;
    
    public MongoCollectionProvider(
        IMongoClient client,
        IOptions<MongoSettings> opts)
    {
        settings = opts.Value;
        db = client.GetDatabase(settings.DatabaseName);
    }
    
    public IMongoCollection<TDocument> GetCollection<TDocument>()
    {
        string typeName = typeof(TDocument).Name;
        
        if (!settings.CollectionNames.TryGetValue(typeName, out string? collName))
            throw new InvalidOperationException(
                $"No mongo collection name configured for document type '{typeName}'");
        
        return db.GetCollection<TDocument>(collName);
    }
}
