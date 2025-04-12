using Game.Core.Equipment;
using Game.Core.Models;
using Game.Core.Rewards;
using Game.Features.Battle.Hubs;
using Game.Features.Battle.PVE.Events;
using Game.Features.Drop;
using Game.Features.Players;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Game.Features.Battle.PVE.Handlers;

public class MonsterDefeatedHandler : INotificationHandler<MonsterDefeatedEvent>
{
    private readonly IDropService _dropService;
    private readonly IBattleRepository _battleRedisRepository;
    private readonly IPlayersMongoRepository _playersMongoRepository;
    private readonly IHubContext<BattleHub> _hubContext;

    public MonsterDefeatedHandler(IDropService dropService,IBattleRepository battleRedisRepository,IPlayersMongoRepository playersMongoRepository,IHubContext<BattleHub> hubContext)
    {
        _dropService = dropService;
        _battleRedisRepository = battleRedisRepository;
        _playersMongoRepository = playersMongoRepository;
        _hubContext = hubContext;
    }
    public async Task Handle(MonsterDefeatedEvent notification, CancellationToken cancellationToken)
    {
        var playerResult = await _playersMongoRepository.GetById(notification.CombatPlayer.Id);
        
        if (playerResult.IsFailure)
        {
            //TODO: Send error
            return;
        }

        var player = playerResult.Value;

        player.Stats.CurrentHealth = notification.CombatPlayer.Stats.CurrentHealth;

        foreach (var usedItem in notification.CombatPlayer.UsedItems)
        {
            var itemSlots = player.Inventory
                .Where(i => i.Item.Id == usedItem.Key)
                .ToArray();

            var remainingToUse = usedItem.Value;

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

        var dropResult = await _dropService.GenerateDrop(notification.Monster);

        if (dropResult.IsFailure)
        {
            //TODO: Send error
            return;
        }

        var drop = dropResult.Value;

        var reward = new BattleReward(){ Gold = 20, Experience = 5,};

        if (drop is not null)
        {
            player.AddToInventory(drop);

            if (drop is EquipmentBase equipment)
            {
                reward.EquipmentDrop = [equipment];
            }
            else
            {
                reward.Drop = [drop];
            }
        }

        if (notification.CombatPlayer.BattleId == null)
        {
            throw new Exception("Cant receive reward,player is not in battle");
        }
            
        await _battleRedisRepository.RemoveBattle(notification.CombatPlayer.BattleId);

        player.BattleId = null;
            
        await _playersMongoRepository.UpdateAsync(player);

        await _hubContext.Clients.User(notification.CombatPlayer.Id).SendAsync("ReceiveBattleReward", reward, cancellationToken: cancellationToken);
    }
}