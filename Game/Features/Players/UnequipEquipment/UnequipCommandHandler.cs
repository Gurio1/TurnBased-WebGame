using Game.Core.Common;
using Game.Core.Models;
using Game.Data.Mongo;
using MongoDB.Driver;

namespace Game.Features.Players.UnequipEquipment;

public sealed class UnequipCommandHandler : IRequestHandler<UnequipCommand,ResultWithoutValue>
{
    private readonly IMongoCollection<Player> collection;
    
    public UnequipCommandHandler(IMongoCollectionProvider<Player> collectionProvider) => collection = collectionProvider.Collection;
    
    public async Task<ResultWithoutValue> Handle(UnequipCommand request, CancellationToken cancellationToken)
    {
        var player = await collection.Find(p => p.Id == request.PlayerId).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        
        if (player is null)
        {
            return ResultWithoutValue.NotFound($"Player with id '{request.PlayerId}' was not found");
        }
        
        var result = player.Unequip(request.EquipmentSlot);
        
        if (result.IsFailure) return result;
        
        var updateDef = PlayerUpdateBuilder.Build(player);
        
        var updateResult = await collection.UpdateOneAsync(p => p.Id == player.Id, updateDef, cancellationToken: cancellationToken);
        
        return updateResult.MatchedCount == 0
            ? ResultWithoutValue.NotFound($"Player '{player.Id}' not found during update.")
            : ResultWithoutValue.Success();
    }
}
