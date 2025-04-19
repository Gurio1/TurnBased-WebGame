using Game.Core;
using Game.Core.Common;
using Game.Core.Equipment;
using Game.Data.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Game.Features.EquipmentBlueprints;

public class EquipmentTemplateMongoRepository : IEquipmentTemplateMongoRepository
{
    private readonly IMongoCollection<EquipmentBlueprint> templatesCollection;

    public EquipmentTemplateMongoRepository(IOptions<MongoSettings> mongoDatabaseSettings,
        IMongoClient mongoClient)
    {

        var mongoDatabase = mongoClient.GetDatabase(
            mongoDatabaseSettings.Value.DatabaseName);

        templatesCollection = mongoDatabase.GetCollection<EquipmentBlueprint>(
            mongoDatabaseSettings.Value.EquipmentTemplatesCollectionName);
    }

    public async Task<Result<EquipmentBlueprint>> GetByEquipmentIdAsync(string equipmentId)
    {
        if (string.IsNullOrEmpty(equipmentId))
        {
            return Result<EquipmentBlueprint>.Invalid("Equipment ID must be provided");
        }

        try
        {
            var template = await templatesCollection.Find(t => t.EquipmentId == equipmentId).FirstOrDefaultAsync();
            return template is null
                ? Result<EquipmentBlueprint>.NotFound($"Equipment template with id '{equipmentId}' was not found")
                : Result<EquipmentBlueprint>.Success(template);
        }
        catch (Exception ex)
        {
            // TODO: Add logging
            return Result<EquipmentBlueprint>.Failure($"An error occurred while retrieving the equipment template: {ex.Message}");
        }
    }

    public async Task SaveAsync(EquipmentBlueprint equipmentBlueprint) => 
        await templatesCollection.InsertOneAsync(equipmentBlueprint);
}
