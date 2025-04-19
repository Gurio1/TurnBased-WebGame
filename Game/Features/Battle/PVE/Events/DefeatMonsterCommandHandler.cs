using Game.Core.Common;
using Game.Core.Equipment;
using Game.Core.Rewards;
using Game.Features.Battle.Hubs;
using Game.Features.Drop;
using Game.Features.Players;
using Microsoft.AspNetCore.SignalR;

namespace Game.Features.Battle.PVE.Events;

public class DefeatMonsterCommandHandler : IRequestHandler<DefeatMonsterCommand, ResultWithoutValue>
{
    private readonly IBattleRepository battleRedisRepository;
    private readonly IDropService dropService;
    private readonly IHubContext<BattleHub> hubContext;
    private readonly IPlayersMongoRepository playersMongoRepository;
    
    public DefeatMonsterCommandHandler(IDropService dropService, IBattleRepository battleRedisRepository,
        IPlayersMongoRepository playersMongoRepository, IHubContext<BattleHub> hubContext)
    {
        this.dropService = dropService;
        this.battleRedisRepository = battleRedisRepository;
        this.playersMongoRepository = playersMongoRepository;
        this.hubContext = hubContext;
    }
    
    public async Task<ResultWithoutValue> Handle(DefeatMonsterCommand notification, CancellationToken cancellationToken)
    {
        var playerResult = await playersMongoRepository.GetById(notification.CombatPlayer.Id);
        
        if (playerResult.IsFailure) return ResultWithoutValue.CreateError(playerResult.Error);
        
        var player = playerResult.Value;
        
        player.Stats.CurrentHealth = notification.CombatPlayer.Stats.CurrentHealth;
        
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
        
        var dropResult = await dropService.GenerateDrop(notification.Monster);
        
        if (dropResult.IsFailure) return ResultWithoutValue.CreateError(dropResult.Error);
        
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
        
        if (notification.CombatPlayer.BattleId == null)
            return ResultWithoutValue.CreateError(new CustomError("400", "Cant receive reward,player is not in battle"));
        
        await battleRedisRepository.RemoveBattle(notification.CombatPlayer.BattleId);
        
        player.BattleId = null;
        
        await playersMongoRepository.UpdateAsync(player);
        
        await hubContext.Clients.User(notification.CombatPlayer.Id)
            .SendAsync("ReceiveBattleReward", reward, cancellationToken);
        
        return ResultWithoutValue.Success();
    }
}
