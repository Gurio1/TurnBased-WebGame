namespace Game.Battle.Persistence.Mongo;

public class MongoSettings
{
    public string DatabaseName { get; set; } = default!;

    public Dictionary<string, string> CollectionNames { get; set; } = new();
}
