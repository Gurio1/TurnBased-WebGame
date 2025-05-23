using Game.Application.SharedKernel;
using Game.Core.Battle;
using Game.Core.Battle.PVE.Events;
using Game.Core.PlayerProfile;
using Game.Persistence.Mongo;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;

namespace Game.Features.Battle.PVE.EventHandlers;

public class BattleLost : INotificationHandler<PveBattleLost>
{
    private readonly IMongoCollection<Player> collection;
    private readonly IHubContext<PveBattleHub, IPveBattleClient> hubContext;
    private readonly IPlayerRepository playerRepository;
    private readonly IBattleRepository battleRepository;
    
    public BattleLost(IHubContext<PveBattleHub, IPveBattleClient> hubContext,
        IMongoCollectionProvider provider, IPlayerRepository playerRepository, IBattleRepository battleRepository)
    {
        this.hubContext = hubContext;
        this.playerRepository = playerRepository;
        this.battleRepository = battleRepository;
        collection = provider.GetCollection<Player>();
    }
    
    //TODO : Add retry logic like BattleLoseFailed
    public async Task Handle(PveBattleLost notification, CancellationToken ct = default)
    {
        var playerResult = await playerRepository.GetById(notification.CombatPlayer.Id, ct);
        
        if (playerResult.IsFailure) return;
        
        var player = playerResult.Value;
        
        if (!player.InBattle()) return;
        
        player.Inventory.RemoveUsedItems(notification.CombatPlayer.UsedItems);
        
        player.ResetBattleId();
        
        player.Stats.CurrentHealth = player.Stats.MaxHealth;
        
        var updateResult = await UpdatePlayer(player);
        
        if (updateResult.IsFailure) return;
        
        var deleteBattleResult = await battleRepository.Delete(player.BattleId!);
        
        if (deleteBattleResult.IsFailure)
        {
            //TODO: log. Add re-try
        }
        
        await hubContext.Clients.User(player.Id).BattleLose(true);
    }
    
    private async Task<ResultWithoutValue> UpdatePlayer(Player player)
    {
        var update = Builders<Player>.Update
            .Set(p => p.Stats, player.Stats)
            .Set(p => p.BattleId, player.BattleId);
        
        var result = await collection.UpdateOneAsync(p => p.Id == player.Id, update);
        
        if (result.MatchedCount == 0)
            return ResultWithoutValue.Failure($"Player with id '{player.Id}' not found");
        
        if (result.ModifiedCount == 0)
            return ResultWithoutValue.Failure($"No changes detected for player '{player.Id}'. " +
                                              $"Inventory/Stats/BattleId may be identical to existing values.");
        
        return ResultWithoutValue.Success();
    }
}
