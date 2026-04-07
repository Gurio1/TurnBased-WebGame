using Game.Core.Equipment;
using Game.Persistence.Mongo;
using Game.SharedKernel.Results;
using MongoDB.Driver;

namespace Game.Persistence.Repositories;

public sealed class EquipmentBlueprintMongoRepository : IEquipmentBlueprintRepository
{
    private readonly IMongoCollection<EquipmentBlueprint> collection;

    public EquipmentBlueprintMongoRepository(IMongoCollectionProvider provider)
    {
        collection = provider.GetCollection<EquipmentBlueprint>();
        CreateIndexes();
    }

    public async Task<Result<IReadOnlyCollection<EquipmentBlueprint>>> GetAll(CancellationToken ct = default)
    {
        var blueprints = await collection.Find(FilterDefinition<EquipmentBlueprint>.Empty).ToListAsync(ct);
        return Result<IReadOnlyCollection<EquipmentBlueprint>>.Success(blueprints);
    }

    public async Task<Result<EquipmentBlueprint>> GetById(string blueprintId, CancellationToken ct = default)
    {
        var blueprint = await collection
            .Find(item => item.Id == blueprintId)
            .FirstOrDefaultAsync(ct);

        return blueprint is null
            ? Result<EquipmentBlueprint>.NotFound($"Equipment blueprint with id '{blueprintId}' was not found.")
            : Result<EquipmentBlueprint>.Success(blueprint);
    }

    public async Task<Result<EquipmentBlueprint>> GetByEquipmentId(string equipmentId, CancellationToken ct = default)
    {
        var blueprint = await collection
            .Find(item => item.EquipmentId == equipmentId)
            .FirstOrDefaultAsync(ct);

        return blueprint is null
            ? Result<EquipmentBlueprint>.NotFound($"Equipment blueprint for equipment '{equipmentId}' was not found.")
            : Result<EquipmentBlueprint>.Success(blueprint);
    }

    public async Task<Result<EquipmentBlueprint>> Create(EquipmentBlueprint blueprint, CancellationToken ct = default)
    {
        try
        {
            await collection.InsertOneAsync(blueprint, cancellationToken: ct);
            return Result<EquipmentBlueprint>.Success(blueprint);
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            return Result<EquipmentBlueprint>.Invalid($"Equipment '{blueprint.EquipmentId}' is already assigned to another blueprint.");
        }
        catch (Exception ex)
        {
            return Result<EquipmentBlueprint>.Failure(ex.Message);
        }
    }

    public async Task<ResultWithoutValue> Update(EquipmentBlueprint blueprint, CancellationToken ct = default)
    {
        try
        {
            var result = await collection
                .ReplaceOneAsync(item => item.Id == blueprint.Id, blueprint, cancellationToken: ct);

            if (result.MatchedCount == 0)
                return ResultWithoutValue.NotFound($"Unable to find EquipmentBlueprint with id '{blueprint.Id}'");

            return ResultWithoutValue.Success();
        }
        catch (MongoWriteException ex) when (ex.WriteError.Category == ServerErrorCategory.DuplicateKey)
        {
            return ResultWithoutValue.Invalid($"Equipment '{blueprint.EquipmentId}' is already assigned to another blueprint.");
        }
    }

    public async Task<ResultWithoutValue> Delete(string blueprintId, CancellationToken ct = default)
    {
        var deleteResult = await collection
            .DeleteOneAsync(item => item.Id == blueprintId, ct);

        return deleteResult.DeletedCount == 0
            ? ResultWithoutValue.NotFound($"Can't delete equipment blueprint with id '{blueprintId}'. Not found.")
            : ResultWithoutValue.Success();
    }

    public Task<bool> IsEquipmentAssigned(string equipmentId, string? excludedBlueprintId = null, CancellationToken ct = default)
    {
        var filter = Builders<EquipmentBlueprint>.Filter.Eq(item => item.EquipmentId, equipmentId);

        if (!string.IsNullOrWhiteSpace(excludedBlueprintId))
        {
            filter &= Builders<EquipmentBlueprint>.Filter.Ne(item => item.Id, excludedBlueprintId);
        }

        return collection.Find(filter).AnyAsync(ct);
    }

    private void CreateIndexes()
    {
        var indexKeys = Builders<EquipmentBlueprint>.IndexKeys.Ascending(item => item.EquipmentId);
        var options = new CreateIndexOptions { Unique = true, Name = "ux_equipment_blueprints_equipmentId" };
        var model = new CreateIndexModel<EquipmentBlueprint>(indexKeys, options);
        collection.Indexes.CreateOne(model);
    }
}
