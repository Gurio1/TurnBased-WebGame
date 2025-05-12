using Game.Core.Equipment;
using Game.Core.SharedKernel;
using Game.Persistence.Mongo;
using MongoDB.Driver;

namespace Game.Features.Equipment.Blueprints.Create;

public sealed class CommandHandler : IRequestHandler<Command, Result<EquipmentBlueprint>>
{
    private readonly IMongoCollection<EquipmentBlueprint> collection;
    
    public CommandHandler(IMongoCollectionProvider provider) => collection = provider.GetCollection<EquipmentBlueprint>();
    
    public async Task<Result<EquipmentBlueprint>> Handle(Command request, CancellationToken cancellationToken)
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
