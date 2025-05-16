using Game.Core.Equipment;
using Game.Core.Loot;
using Game.Core.Models;
using Game.Core.Rewards;
using Game.Core.SharedKernel;
using Game.Persistence.Mongo;
using Game.Persistence.Repositories;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;

namespace Game.Features.Battle.PVE.Commands;

public class DefeatMonsterCommandHandler : IRequestHandler<DefeatMonsterCommand, ResultWithoutValue>
{
    private readonly IMongoCollection<Player> collection;
    private readonly IHubContext<PveBattleHub> hubContext;
    private readonly ILootService lootService;
    private readonly IPlayerRepository playerRepository;
    
    public DefeatMonsterCommandHandler(ILootService lootService, IHubContext<PveBattleHub> hubContext,
        IPlayerRepository playerRepository, IMongoCollectionProvider provider)
    {
        this.lootService = lootService;
        this.hubContext = hubContext;
        this.playerRepository = playerRepository;
        collection = provider.GetCollection<Player>();
    }
    
    public async Task<ResultWithoutValue> Handle(DefeatMonsterCommand notification, CancellationToken cancellationToken)
    {
        var playerResult = await playerRepository.GetById(notification.CombatPlayer.Id,cancellationToken);
        
        if (playerResult.IsFailure)
            return ResultWithoutValue.CreateError(playerResult.Error);
        
        var player = playerResult.Value;
        
        if (player.BattleId == null)
            return ResultWithoutValue.Invalid("Cant receive reward,player is not in battle");
        
        player.Stats = notification.CombatPlayer.Stats;
        
        player.RemoveUsedItems(notification.CombatPlayer.UsedItems);
        
        var dropResult = await lootService.GenerateDrop(notification.Monster);
        
        if (dropResult.IsFailure)
            return ResultWithoutValue.CreateError(dropResult.Error);
        
        var drop = dropResult.Value;
        
        var reward = new BattleReward { Gold = 20, Experience = 5 };
        
        if (drop is not null)
        {
            player.AddToInventory(drop);
            
            if (drop is EquipmentBase equipment)
                reward.EquipmentDrop = [equipment];
            else
                reward.Drop = [drop];
        }
        
        player.BattleId = null;
        
        var updateResult = await UpdatePlayer(player);
        
        if (updateResult.IsFailure)
            return updateResult;
        
        await hubContext.Clients.User(notification.CombatPlayer.Id)
            .SendAsync("ReceiveBattleReward", reward, cancellationToken);
        
        return ResultWithoutValue.Success();
    }
    
    private async Task<ResultWithoutValue> UpdatePlayer(Player player)
    {
        var update = Builders<Player>.Update
            .Set(p => p.Inventory, player.Inventory)
            .Set(p => p.Stats, player.Stats)
            .Set(p => p.BattleId, player.BattleId);
        
        var result = await collection.UpdateOneAsync(p => p.Id == player.Id, update);
        
        if (result.MatchedCount == 0 )
        {
            return ResultWithoutValue.Failure($"Can't find player with id '{player.Id}'");
        }
        
        return result.ModifiedCount > 0
            ? ResultWithoutValue.Success()
            : ResultWithoutValue.Failure($"Can not update player with id '{player.Id}'");
    }
}
