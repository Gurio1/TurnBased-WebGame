using Game.Data.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Game.Core.Equipment;

public class EquipmentService
{
    private readonly IMongoCollection<EquipmentBase> _equipmentCollection;

    //TODO : Create generic class for this
    public EquipmentService(IOptions<MongoSettings> mongoDatabaseSettings)
    {
        var mongoClient = new MongoClient(
            mongoDatabaseSettings.Value.ConnectionString);

        var mongoDatabase = mongoClient.GetDatabase(
            mongoDatabaseSettings.Value.DatabaseName);

        _equipmentCollection = mongoDatabase.GetCollection<EquipmentBase>(
            mongoDatabaseSettings.Value.EquipmentCollectionName);
    }

    public async Task<EquipmentBase?> GetById(string id)
    {
        return await _equipmentCollection.Find(t => t.Id == id).FirstOrDefaultAsync();
    }
    
    public async Task SaveAsync(EquipmentBase equipment)
    { 
        await _equipmentCollection.InsertOneAsync(equipment);
    }
}