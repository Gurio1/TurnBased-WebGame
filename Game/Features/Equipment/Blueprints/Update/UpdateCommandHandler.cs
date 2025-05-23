using Game.Application.SharedKernel;
using Game.Core.Equipment;
using Game.Persistence.Mongo;
using MongoDB.Driver;

namespace Game.Features.Equipment.Blueprints.Update;

public sealed class UpdateCommandHandler : IRequestHandler<UpdateCommand, ResultWithoutValue>
{
    private readonly IMongoCollection<EquipmentBlueprint> collection;
    
    public UpdateCommandHandler(IMongoCollectionProvider provider) =>
        collection = provider.GetCollection<EquipmentBlueprint>();
    
    public async Task<ResultWithoutValue> Handle(UpdateCommand request, CancellationToken cancellationToken)
    {
        var result = await collection.ReplaceOneAsync(x => x.Id == request.Blueprint.Id,
            request.Blueprint, cancellationToken: cancellationToken);
        
        if (result.MatchedCount == 0)
            return ResultWithoutValue.NotFound($"Unable to find EquipmentBlueprint with id '{request.Blueprint.Id}'");
        
        return result.ModifiedCount > 0
            ? ResultWithoutValue.Success()
            : ResultWithoutValue.Failure(
                $"Error occured while updating EquipmentBlueprint with id '{request.Blueprint.Id}'");
    }
}
