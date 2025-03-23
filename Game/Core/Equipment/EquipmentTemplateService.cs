using Game.Data.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Game.Core.Equipment;

public class EquipmentTemplateService
{
    private readonly IMongoCollection<EquipmentTemplate> _templatesCollection;

    //TODO : Create generic class for this
    public EquipmentTemplateService(IOptions<MongoSettings> mongoDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            mongoDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            mongoDatabaseSettings.Value.DatabaseName);

        _templatesCollection = mongoDatabase.GetCollection<EquipmentTemplate>(
            mongoDatabaseSettings.Value.EquipmentTemplatesCollectionName);
    }

    public async Task<EquipmentTemplate?> GetByEquipmentIdAsync(string equipmentId)
    {
        return await _templatesCollection.Find(t => t.EquipmentId == equipmentId).FirstOrDefaultAsync();
    }

    public async Task SaveAsync(EquipmentTemplate template)
    { 
        await _templatesCollection.InsertOneAsync(template);
    }
}