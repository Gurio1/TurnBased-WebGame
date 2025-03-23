namespace Game.Data.Mongo;

public class MongoSettings
{
    public string ConnectionString { get; set; } = default!;

    public string DatabaseName { get; set; } = default!;

    public string PlayersCollectionName { get; set; } = default!;
    public string AbilitiesCollectionName { get; set; } = default!;
    public string EquipmentCollectionName { get; set; } = default!;
    public string EquipmentTemplatesCollectionName { get; set; } = default!;
}