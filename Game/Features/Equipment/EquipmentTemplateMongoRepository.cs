using Game.Core;
using Game.Core.Equipment;
using Game.Data.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Game.Features.Equipment;

public class EquipmentTemplateMongoRepository : IEquipmentTemplateMongoRepository
{
    private readonly IMongoCollection<EquipmentTemplate> _templatesCollection;

    public EquipmentTemplateMongoRepository(IOptions<MongoSettings> mongoDatabaseSettings,
        IMongoClient mongoClient)
    {

        var mongoDatabase = mongoClient.GetDatabase(
            mongoDatabaseSettings.Value.DatabaseName);

        _templatesCollection = mongoDatabase.GetCollection<EquipmentTemplate>(
            mongoDatabaseSettings.Value.EquipmentTemplatesCollectionName);
    }

    public async Task<Result<EquipmentTemplate>> GetByEquipmentIdAsync(string equipmentId)
    {
        if (string.IsNullOrEmpty(equipmentId))
        {
            return Result<EquipmentTemplate>.Invalid("Equipment ID must be provided"); 
        }
        
        try
        {
            var template = await _templatesCollection.Find(t => t.EquipmentId == equipmentId).FirstOrDefaultAsync();
            return template is null
                ? Result<EquipmentTemplate>.NotFound($"Equipment template with id '{equipmentId}' was not found")
                : Result<EquipmentTemplate>.Success(template);
        }
        catch (Exception ex)
        {
            // TODO: Add logging 
            return Result<EquipmentTemplate>.Failure($"An error occurred while retrieving the equipment template: {ex.Message}");
        }
    }

    public async Task SaveAsync(EquipmentTemplate template)
    { 
        await _templatesCollection.InsertOneAsync(template);
    }
}