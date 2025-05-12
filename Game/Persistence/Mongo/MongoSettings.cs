namespace Game.Persistence.Mongo;

public class MongoSettings
{
    public string DatabaseName { get; set; } = default!;
    
    /// <summary>
    ///     Maps the name of document type (e.g. "EquipmentBlueprint")
    ///     to the corresponding collection name in Mongo.
    /// </summary>
    public Dictionary<string, string> CollectionNames { get; set; }
        = new();
}
