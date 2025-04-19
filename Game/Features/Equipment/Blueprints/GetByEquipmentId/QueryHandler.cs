using Game.Core.Common;
using Game.Core.Equipment;
using Game.Data.Mongo;
using MongoDB.Driver;

namespace Game.Features.Equipment.Blueprints.GetByEquipmentId;

public sealed class QueryHandler : IRequestHandler<Query, Result<EquipmentBlueprint>>
{
    private readonly IMongoCollection<EquipmentBlueprint> collection;
    
    public QueryHandler(IMongoCollectionProvider<EquipmentBlueprint> provider) => collection = provider.Collection;
    
    public async Task<Result<EquipmentBlueprint>> Handle(Query request,
        CancellationToken cancellationToken)
    {
        try
        {
            var template = await collection
                .Find(t => t.EquipmentId == request.EquipmentId)
                .FirstOrDefaultAsync(cancellationToken);
            
            return template is null
                ? Result<EquipmentBlueprint>.NotFound(
                    $"Equipment template with id '{request.EquipmentId}' was not found")
                : Result<EquipmentBlueprint>.Success(template);
        }
        catch (Exception ex)
        {
            // TODO: Add logging
            return Result<EquipmentBlueprint>.Failure(
                $"An error occurred while retrieving the equipment template: {ex.Message}");
        }
    }
}
