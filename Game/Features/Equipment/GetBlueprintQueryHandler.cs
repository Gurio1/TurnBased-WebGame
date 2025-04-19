using Game.Core.Common;
using Game.Core.Equipment;
using Game.Data.Mongo;
using MongoDB.Driver;

namespace Game.Features.Equipment;

public sealed class GetBlueprintQueryHandler : IRequestHandler<GetBlueprintQuery, Result<BlueprintAttributes>>
{
    private readonly IMongoCollection<EquipmentBlueprint> collection;
    
    public GetBlueprintQueryHandler(IMongoCollectionProvider<EquipmentBlueprint> provider) =>
        collection = provider.Collection;
    
    public async Task<Result<BlueprintAttributes>> Handle(GetBlueprintQuery request,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(request.EquipmentId))
            return Result<BlueprintAttributes>.Invalid("Equipment ID must be provided");
        
        try
        {
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
        catch (Exception ex)
        {
            // TODO: Add logging
            return Result<BlueprintAttributes>.Failure(
                $"An error occurred while retrieving the equipment template: {ex.Message}");
        }
    }
}
