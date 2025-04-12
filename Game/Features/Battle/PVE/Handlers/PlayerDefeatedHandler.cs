using Game.Features.Battle.Hubs;
using Game.Features.Battle.PVE.Events;
using Game.Features.Players;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Game.Features.Battle.PVE.Handlers;

public class PlayerDefeatedHandler : INotificationHandler<PlayerDefeatedEvent>
{
    private readonly IBattleRepository _battleRedisRepository;
    private readonly IPlayersMongoRepository _playersMongoRepository;
    private readonly IHubContext<BattleHub> _hubContext;

    public PlayerDefeatedHandler(IBattleRepository battleRedisRepository,IPlayersMongoRepository playersMongoRepository,IHubContext<BattleHub> hubContext)
    {
        _battleRedisRepository = battleRedisRepository;
        _playersMongoRepository = playersMongoRepository;
        _hubContext = hubContext;
    }
    public async Task Handle(PlayerDefeatedEvent notification, CancellationToken cancellationToken)
    {
        var playerResult = await _playersMongoRepository.GetById(notification.CombatPlayer.Id);

        if (playerResult.IsFailure)
        {
            //TODO: Send error
            return;
        }

        var player = playerResult.Value;
        
        if (!player.InBattle())
            throw new Exception($"Player is not in battle - {nameof(PlayerDefeatedHandler)}");

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


        await _battleRedisRepository.RemoveBattle(notification.CombatPlayer.BattleId!);
        
        player.BattleId = null;
        player.Stats.CurrentHealth = player.Stats.MaxHealth;
            
        await _playersMongoRepository.UpdateAsync(player);
        
        await _hubContext.Clients.User(player.Id).SendAsync("ReceiveBattleLose", true,cancellationToken: cancellationToken);
    }
}