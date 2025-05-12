using Game.Application.Features.Battle.PVE;
using Game.Core.Models;
using Game.Core.SharedKernel;
using Game.Features.Players;
using Game.Persistence.Mongo;
using Game.Persistence.Repositories;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;

namespace Game.Features.Battle.PVE.Events;

public class DefeatPlayerCommandHandler : IRequestHandler<DefeatPlayerCommand, ResultWithoutValue>
{
    private readonly IHubContext<PveBattleHub> hubContext;
    private readonly IPlayerRepository playerRepository;
    private readonly IMongoCollection<Player> collection;
    
    public DefeatPlayerCommandHandler( IHubContext<PveBattleHub> hubContext, IMongoCollectionProvider provider,
        IPlayerRepository playerRepository)
    {
        this.hubContext = hubContext;
        this.playerRepository = playerRepository;
        collection = provider.GetCollection<Player>();
    }
    
    public async Task<ResultWithoutValue> Handle(DefeatPlayerCommand notification, CancellationToken cancellationToken)
    {
        var playerResult = await playerRepository.GetById(notification.CombatPlayer.Id);
        
        if (playerResult.IsFailure) return ResultWithoutValue.CreateError(playerResult.Error);
        
        var player = playerResult.Value;
        
        if (!player.InBattle())
            throw new InvalidOperationException($"Player is not in battle - {nameof(DefeatPlayerCommandHandler)}");
        
        foreach (var usedItem in notification.CombatPlayer.UsedItems)
        {
            var itemSlots = player.Inventory
                .Where(i => i.Item.Id == usedItem.Key)
                .ToArray();
            
            int remainingToUse = usedItem.Value;
            
            foreach (var slot in itemSlots)
            {
                if (remainingToUse <= 0)
                    break;
                
                if (slot.Quantity > remainingToUse)
                {
                    slot.Quantity -= remainingToUse;
                    remainingToUse = 0;
                }
                else
                {
                    remainingToUse -= slot.Quantity;
                    player.RemoveSlotFromInventory(slot);
                }
            }
        }
        
        player.BattleId = null;
        player.Stats.CurrentHealth = player.Stats.MaxHealth;
        
        var updateResult = await UpdatePlayer(player);
        
        if (updateResult.IsFailure)
        {
            return updateResult;
        }
        
        await hubContext.Clients.User(player.Id).SendAsync("ReceiveBattleLose", true, cancellationToken);
        
        return ResultWithoutValue.Success();
    }
    
    private async Task<ResultWithoutValue> UpdatePlayer(Player player)
    {
        var update = Builders<Player>.Update
            .Set(p => p.Stats , player.Stats)
            .Set(p => p.BattleId, player.BattleId);
        
        var result = await collection.UpdateOneAsync(p => p.Id == player.Id, update);
        
        if (result.MatchedCount == 0)
        {
            return ResultWithoutValue.Failure($"Player with id '{player.Id}' not found");
        }
        
        if (result.ModifiedCount == 0)
        {
            var original = await collection.Find(p => p.Id == player.Id).FirstOrDefaultAsync();
            return ResultWithoutValue.Failure($"No changes detected for player '{player.Id}'. " +
                                              $"Inventory/Stats/BattleId may be identical to existing values.");
        }
        
        return ResultWithoutValue.Success();
    }
}
