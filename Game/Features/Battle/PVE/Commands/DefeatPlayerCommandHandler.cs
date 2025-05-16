using Game.Core.Models;
using Game.Core.SharedKernel;
using Game.Persistence.Mongo;
using Game.Persistence.Repositories;
using Microsoft.AspNetCore.SignalR;
using MongoDB.Driver;

namespace Game.Features.Battle.PVE.Commands;

public class DefeatPlayerCommandHandler : IRequestHandler<DefeatPlayerCommand, ResultWithoutValue>
{
    private readonly IMongoCollection<Player> collection;
    private readonly IHubContext<PveBattleHub> hubContext;
    private readonly IPlayerRepository playerRepository;
    
    public DefeatPlayerCommandHandler(IHubContext<PveBattleHub> hubContext, IMongoCollectionProvider provider,
        IPlayerRepository playerRepository)
    {
        this.hubContext = hubContext;
        this.playerRepository = playerRepository;
        collection = provider.GetCollection<Player>();
    }
    
    public async Task<ResultWithoutValue> Handle(DefeatPlayerCommand notification, CancellationToken cancellationToken)
    {
        var playerResult = await playerRepository.GetById(notification.CombatPlayer.Id,cancellationToken);
        
        if (playerResult.IsFailure) return ResultWithoutValue.CreateError(playerResult.Error);
        
        var player = playerResult.Value;
        
        if (!player.InBattle())
            throw new InvalidOperationException($"Player is not in battle - {nameof(DefeatPlayerCommandHandler)}");
        
        player.RemoveUsedItems(notification.CombatPlayer.UsedItems);
        
        player.BattleId = null;
        player.Stats.CurrentHealth = player.Stats.MaxHealth;
        
        var updateResult = await UpdatePlayer(player);
        
        if (updateResult.IsFailure) return updateResult;
        
        await hubContext.Clients.User(player.Id).SendAsync("ReceiveBattleLose", true, cancellationToken);
        
        return ResultWithoutValue.Success();
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
