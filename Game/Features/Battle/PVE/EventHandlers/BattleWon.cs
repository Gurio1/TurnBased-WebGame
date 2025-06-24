using Game.Application.SharedKernel;
using Game.Core.Battle;
using Game.Core.Battle.PVE.Events;
using Game.Core.Equipment;
using Game.Core.Loot;
using Game.Core.PlayerProfile;
using Game.Core.PlayerProfile.Aggregates;
using Game.Core.Rewards;
using Game.Persistence.Mongo;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;

namespace Game.Features.Battle.PVE.EventHandlers;

public class BattleWon : INotificationHandler<PveBattleWon>
{
    private readonly IMongoCollection<GamePlayer> collection;
    private readonly IHubContext<PveBattleHub, IPveBattleClient> hubContext;
    private readonly ILootService lootService;
    private readonly IPlayerRepository playerRepository;
    private readonly IBattleRepository battleRepository;
    
    public BattleWon(ILootService lootService, IHubContext<PveBattleHub, IPveBattleClient> hubContext,
        IPlayerRepository playerRepository, IMongoCollectionProvider provider, IBattleRepository battleRepository)
    {
        this.lootService = lootService;
        this.hubContext = hubContext;
        this.playerRepository = playerRepository;
        this.battleRepository = battleRepository;
        collection = provider.GetCollection<GamePlayer>();
    }
    
    public async Task Handle(PveBattleWon notification, CancellationToken ct = default)
    {
        var playerResult = await playerRepository.GetById(notification.CombatPlayer.Id, ct);
        
        if (playerResult.IsFailure)
        {
            await hubContext.Clients.User(notification.CombatPlayer.Id)
                .BattleErrorMessage(playerResult.Error.Description);
            return;
        }
        
        
        var player = playerResult.Value;
        
        if (player.BattleId == null)
        {
            await hubContext.Clients.User(notification.CombatPlayer.Id)
                .BattleErrorMessage("Cant receive reward,player is not in battle");
            return;
        }
        
        player.Stats = notification.CombatPlayer.Stats;
        
        player.Inventory.RemoveUsedItems(notification.CombatPlayer.UsedItems);
        
        var dropResult = await lootService.GenerateDrop(notification.Monster);
        
        if (dropResult.IsFailure)
        {
            await hubContext.Clients.User(player.Id)
                .BattleErrorMessage(dropResult.Error.Description);
            return;
        }
        
        var drop = dropResult.Value;
        
        //TODO: Move to a class
        var reward = new BattleReward { Gold = 20, Experience = 5 };
        
        if (drop is not null)
        {
            player.Inventory.Add(drop);
            
            if (drop is EquipmentBase equipment)
                reward.EquipmentDrop = [equipment];
            else
                reward.Drop = [drop];
        }
        
        player.ResetBattleId();
        
        var updateResult = await UpdatePlayer(player);
        
        if (updateResult.IsFailure)
        {
            await hubContext.Clients.User(player.Id)
                .BattleErrorMessage(updateResult.Error.Description);
            return;
        }
        
        var removeBattleResult = await battleRepository.Delete(notification.CombatPlayer.BattleId);
        
        if (removeBattleResult.IsFailure)
        {
            //TODO: log. Add re-try
        }
        
        await hubContext.Clients.User(notification.CombatPlayer.Id)
            .BattleReward(reward);
    }
    
    private async Task<ResultWithoutValue> UpdatePlayer(GamePlayer gamePlayer)
    {
        var update = Builders<GamePlayer>.Update
            .Set(p => p.Inventory, gamePlayer.Inventory)
            .Set(p => p.Stats, gamePlayer.Stats)
            .Set(p => p.BattleId, gamePlayer.BattleId);
        
        var result = await collection.UpdateOneAsync(p => p.Id == gamePlayer.Id, update);
        
        if (result.MatchedCount == 0) return ResultWithoutValue.Failure($"Can't find player with id '{gamePlayer.Id}'");
        
        return result.ModifiedCount > 0
            ? ResultWithoutValue.Success()
            : ResultWithoutValue.Failure($"Can not update player with id '{gamePlayer.Id}'");
    }
}
