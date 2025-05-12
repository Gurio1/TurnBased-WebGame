using Game.Core.Equipment;
using Game.Core.SharedKernel;
using Game.Persistence.Mongo;
using MongoDB.Driver;

namespace Game.Features.Equipment.Blueprints.GetByEquipmentId;

public sealed class GetQueryHandler : IRequestHandler<GetQuery, Result<EquipmentBlueprint>>
{
    private readonly IMongoCollection<EquipmentBlueprint> collection;
    
    public GetQueryHandler(IMongoCollectionProvider provider) => collection = provider.GetCollection<EquipmentBlueprint>();
    
    public async Task<Result<EquipmentBlueprint>> Handle(GetQuery request,
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
