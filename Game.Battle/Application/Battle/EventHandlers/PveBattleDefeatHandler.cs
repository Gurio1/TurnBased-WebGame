using Game.Battle.Core.Battle;
using Game.Battle.Core.Battle.PVE.Events;
using Game.Battle.Messaging.Clients;
using Game.Battle.SignalR;
using Game.SharedKernel.Battle;
using Game.SharedKernel.Contracts.Requests;
using Microsoft.AspNetCore.SignalR;

namespace Game.Battle.Application.Battle.EventHandlers;

public sealed class PveBattleDefeatHandler
{
    private readonly IHubContext<PveBattleHub, IPveBattleClient> hubContext;
    private readonly IBattleSettlementClient battleSettlementClient;
    private readonly IBattleRepository battleRepository;
    private readonly PlayerBattleCache playerBattleCache;

    public PveBattleDefeatHandler(IHubContext<PveBattleHub, IPveBattleClient> hubContext,
        IBattleSettlementClient battleSettlementClient, IBattleRepository battleRepository, PlayerBattleCache playerBattleCache)
    {
        this.hubContext = hubContext;
        this.battleSettlementClient = battleSettlementClient;
        this.battleRepository = battleRepository;
        this.playerBattleCache = playerBattleCache;
    }

    public async Task Handle(PveBattleLost notification, CancellationToken ct = default)
    {
        if (notification.Player.BattleId is null)
        {
            await hubContext.Clients.User(notification.Player.Id)
                .BattleErrorMessage("Cant resolve battle loss,player battle id is missing");
            return;
        }

        var resolveResult = await battleSettlementClient.ResolveBattleAsync(new BattleResolveRequest
        {
            PlayerId = notification.Player.Id,
            BattleId = notification.Player.BattleId,
            MonsterName = string.Empty,
            Won = false,
            PlayerStats = notification.Player.Stats,
            UsedItems = notification.Player.UsedItems
        }, ct);

        if (resolveResult.IsFailure)
        {
            await hubContext.Clients.User(notification.Player.Id)
                .BattleErrorMessage(resolveResult.Error.Description);
            return;
        }

        var deleteBattleResult = await battleRepository.Delete(notification.Player.BattleId);

        if (deleteBattleResult.IsFailure)
        {
            //TODO: log. Add re-try
        }

        await playerBattleCache.Remove(notification.Player.Id);

        await hubContext.Clients.User(notification.Player.Id).BattleLose(true);
    }
}
