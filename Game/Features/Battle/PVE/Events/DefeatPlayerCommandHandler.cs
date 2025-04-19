using Game.Core.Common;
using Game.Features.Battle.Hubs;
using Game.Features.Players;
using Microsoft.AspNetCore.SignalR;

namespace Game.Features.Battle.PVE.Events;

public class DefeatPlayerCommandHandler : IRequestHandler<DefeatPlayerCommand,ResultWithoutValue>
{
    private readonly IBattleRepository battleRedisRepository;
    private readonly IPlayersMongoRepository playersMongoRepository;
    private readonly IHubContext<BattleHub> hubContext;

    public DefeatPlayerCommandHandler(IBattleRepository battleRedisRepository,IPlayersMongoRepository playersMongoRepository,IHubContext<BattleHub> hubContext)
    {
        this.battleRedisRepository = battleRedisRepository;
        this.playersMongoRepository = playersMongoRepository;
        this.hubContext = hubContext;
    }
    public async Task<ResultWithoutValue> Handle(DefeatPlayerCommand notification, CancellationToken cancellationToken)
    {
        var playerResult = await playersMongoRepository.GetById(notification.CombatPlayer.Id);

        if (playerResult.IsFailure)
        {
            return ResultWithoutValue.Failure(playerResult.Error);
        }

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


        await battleRedisRepository.RemoveBattle(notification.CombatPlayer.BattleId!);

        player.BattleId = null;
        player.Stats.CurrentHealth = player.Stats.MaxHealth;

        await playersMongoRepository.UpdateAsync(player);

        await hubContext.Clients.User(player.Id).SendAsync("ReceiveBattleLose", true,cancellationToken: cancellationToken);
        
        return ResultWithoutValue.Success();
    }
}
