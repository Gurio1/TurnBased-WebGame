using Game.Battle.Core.Battle;
using Game.Battle.Core.Battle.PVE.Events;
using Game.Battle.Core.Models;
using Game.Battle.Core.Rewards;
using Game.Battle.Messaging.Clients;
using Game.Battle.SignalR;
using Game.SharedKernel.Battle;
using Game.SharedKernel.Contracts.Requests;
using Microsoft.AspNetCore.SignalR;

namespace Game.Battle.Application.Battle.EventHandlers;

public sealed class PveBattleVictoryHandler
{
    private readonly IHubContext<PveBattleHub, IPveBattleClient> hubContext;
    private readonly IBattleSettlementClient battleSettlementClient;
    private readonly IBattleRepository battleRepository;
    private readonly PlayerBattleCache playerBattleCache;

    public PveBattleVictoryHandler(IHubContext<PveBattleHub, IPveBattleClient> hubContext,
        IBattleSettlementClient battleSettlementClient, IBattleRepository battleRepository, PlayerBattleCache playerBattleCache)
    {
        this.hubContext = hubContext;
        this.battleSettlementClient = battleSettlementClient;
        this.battleRepository = battleRepository;
        this.playerBattleCache = playerBattleCache;
    }

    public async Task Handle(PveBattleWon notification, CancellationToken ct = default)
    {
        if (notification.Player.BattleId is null)
        {
            await hubContext.Clients.User(notification.Player.Id)
                .BattleErrorMessage("Cant receive reward,player battle id is missing");
            return;
        }

        var resolveResult = await battleSettlementClient.ResolveBattleAsync(new BattleResolveRequest
        {
            PlayerId = notification.Player.Id,
            BattleId = notification.Player.BattleId,
            MonsterName = notification.Monster.Name,
            Won = true,
            PlayerStats = notification.Player.Stats,
            UsedItems = notification.Player.UsedItems
        }, ct);

        if (resolveResult.IsFailure)
        {
            await hubContext.Clients.User(notification.Player.Id)
                .BattleErrorMessage(resolveResult.Error.Description);
            return;
        }

        var reward = MapReward(resolveResult.Value.Reward);

        var removeBattleResult = await battleRepository.Delete(notification.Player.BattleId);
        if (removeBattleResult.IsFailure)
        {
            //TODO: log. Add re-try
        }

        await playerBattleCache.Remove(notification.Player.Id);

        await hubContext.Clients.User(notification.Player.Id)
            .BattleReward(reward);
    }

    private static BattleReward MapReward(BattleRewardDto? rewardDto)
    {
        if (rewardDto is null)
        {
            return new BattleReward { Gold = 20, Experience = 5 };
        }

        return new BattleReward
        {
            Gold = rewardDto.Gold,
            Experience = rewardDto.Experience,
            Drop = rewardDto.Drop?.Select(item => (Item)new LootItem(item.MaxInventorySlotQuantity)
            {
                Id = item.Id,
                Name = item.Name,
                ImageUrl = item.ImageUrl,
                Quantity = item.Quantity
            }).ToList(),
            EquipmentDrop = rewardDto.EquipmentDrop?.Select(item => new Equipment
            {
                Id = item.Id,
                Name = item.Name,
                ImageUrl = item.ImageUrl,
                EquipmentId = item.EquipmentId,
                Slot = item.Slot
            }).ToList()
        };
    }
}
