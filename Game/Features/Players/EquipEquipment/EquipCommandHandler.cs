using Game.Core.Equipment;
using Game.Core.Models;
using Game.Core.SharedKernel;
using Game.Persistence.Mongo;
using MongoDB.Driver;

namespace Game.Features.Players.EquipEquipment;

//TODO: I dont know.Should i create different commands for getting and updating a player?(This operations are too simple so i dont see any reason)
public sealed class EquipCommandHandler : IRequestHandler<EquipCommand, ResultWithoutValue>
{
    private readonly IMongoCollection<Player> collection;
    
    public EquipCommandHandler(IMongoCollectionProvider provider)
        => collection = provider.GetCollection<Player>();
    
    public async Task<ResultWithoutValue> Handle(EquipCommand request, CancellationToken cancellationToken)
    {
        var player = await collection.Find(a => a.Id == request.PlayerId).FirstOrDefaultAsync(cancellationToken);
        
        if (player is null)
            return ResultWithoutValue.NotFound($"Unable to retrieve player with id '{request.PlayerId}'");
        
        var item = player.Inventory
            .FirstOrDefault(s => s.Item.Id == request.ItemId)?
            .Item;
        
        if (item is null)
            return ResultWithoutValue.NotFound($"Unable to retrieve item with id '{request.ItemId}'");
        
        if (item is not EquipmentBase equipment || !item.CanInteract(ItemInteractions.Equip))
            return ResultWithoutValue.Invalid($"Item '{item?.Name ?? request.ItemId}' doesn't have equip behaviour.");
        
        player.Equip(equipment);
        
        var updateDef = PlayerUpdateBuilder.Build(player);
        
        var result =
            await collection.UpdateOneAsync(p => p.Id == player.Id, updateDef, cancellationToken: cancellationToken);
        
        return result.MatchedCount == 0
            ? ResultWithoutValue.NotFound($"Player '{player.Id}' not found during update.")
            : ResultWithoutValue.Success();
    }
}
