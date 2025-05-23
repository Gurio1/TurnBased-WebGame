using Game.Application.SharedKernel;
using Game.Core.Equipment;
using Game.Persistence.Mongo;
using MongoDB.Driver;

namespace Game.Features.Equipment.Blueprints.GetByEquipmentId;

public sealed class GetQueryHandler : IRequestHandler<GetQuery, Result<EquipmentBlueprint>>
{
    private readonly IMongoCollection<EquipmentBlueprint> collection;
    
    public GetQueryHandler(IMongoCollectionProvider provider) =>
        collection = provider.GetCollection<EquipmentBlueprint>();
    
    public async Task<Result<EquipmentBlueprint>> Handle(GetQuery request,
        CancellationToken cancellationToken)
    {
        var template = await collection
            .Find(t => t.EquipmentId == request.EquipmentId)
            .FirstOrDefaultAsync(cancellationToken);
        
        return template is null
            ? Result<EquipmentBlueprint>.NotFound(
                $"Equipment template with id '{request.EquipmentId}' was not found")
            : Result<EquipmentBlueprint>.Success(template);
    }
}
