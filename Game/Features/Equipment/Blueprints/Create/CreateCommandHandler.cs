using Game.Core.Common;
using Game.Core.Equipment;
using Game.Data.Mongo;
using MongoDB.Driver;

namespace Game.Features.Equipment.Blueprints.Create;

public sealed class CreateCommandHandler : IRequestHandler<CreateCommand, Result<EquipmentBlueprint>>
{
    private readonly IMongoCollection<EquipmentBlueprint> collection;
    
    public CreateCommandHandler(IMongoCollectionProvider<EquipmentBlueprint> provider) => collection = provider.Collection;
    
    public async Task<Result<EquipmentBlueprint>> Handle(CreateCommand request, CancellationToken cancellationToken)
    {
        var newTemplate = new EquipmentBlueprint
        {
            AttributeRanges = request.AttributeRanges,
            EquipmentId = request.EquipmentId,
            AttributeCountWeights = request.AttributeCountWeights
        };
        
        try
        {
            await collection.InsertOneAsync(newTemplate, cancellationToken: cancellationToken);
            return Result<EquipmentBlueprint>.Success(newTemplate);
        }
        //TODO : Should i specify exact exception? e.g MongoWriteException 
        catch (Exception e)
        {
            return Result<EquipmentBlueprint>.Failure(e.Message);
        }
    }
}
