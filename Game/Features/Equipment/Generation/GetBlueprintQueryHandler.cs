using Game.Application.SharedKernel;
using Game.Core.Equipment;
using Game.Core.Equipment.Generation;
using Game.Persistence.Mongo;
using MongoDB.Driver;

namespace Game.Features.Equipment.Generation;

public sealed class GetBlueprintQueryHandler : IRequestHandler<GetBlueprintQuery, Result<BlueprintAttributes>>
{
    private readonly IMongoCollection<EquipmentBlueprint> collection;
    
    public GetBlueprintQueryHandler(IMongoCollectionProvider provider) =>
        collection = provider.GetCollection<EquipmentBlueprint>();
    
    public async Task<Result<BlueprintAttributes>> Handle(GetBlueprintQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.EquipmentId))
            return Result<BlueprintAttributes>.Invalid("Equipment ID must be provided");
        
        var attrs = await collection
            .Find(b => b.EquipmentId == request.EquipmentId)
            .Project(b => new BlueprintAttributes
            {
                AttributeRanges = b.AttributeRanges, AttributeCountWeights = b.AttributeCountWeights
            })
            .FirstOrDefaultAsync(cancellationToken);
        
        return attrs is null
            ? Result<BlueprintAttributes>.NotFound(
                $"Equipment template with equipment id '{request.EquipmentId}' was not found")
            : Result<BlueprintAttributes>.Success(attrs);
    }
}
