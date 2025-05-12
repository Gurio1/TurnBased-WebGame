using Game.Core.Equipment;
using Game.Core.SharedKernel;
using Game.Persistence.Mongo;
using MongoDB.Driver;

namespace Game.Features.Equipment.Blueprints.Delete;

public sealed class DeleteCommandHandler : IRequestHandler<DeleteCommand, ResultWithoutValue>
{
    private readonly IMongoCollection<EquipmentBlueprint> collection;
    
    public DeleteCommandHandler(IMongoCollectionProvider provider) =>
        collection = provider.GetCollection<EquipmentBlueprint>();
    
    public async Task<ResultWithoutValue> Handle(DeleteCommand request, CancellationToken cancellationToken)
    {
        var deleteResult = await collection.DeleteOneAsync(b => b.Id == request.BlueprintId, cancellationToken);
        
        return deleteResult.DeletedCount == 0
            ? ResultWithoutValue.NotFound($"Can't delete EquipmentBlueprint with id '{request.BlueprintId}'. Not found.")
            : ResultWithoutValue.Success();
    }
}
