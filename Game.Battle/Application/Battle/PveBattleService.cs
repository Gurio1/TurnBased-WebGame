using Game.Battle.Contracts;
using Game.Battle.Core.Battle;
using Game.Battle.Core.Battle.PVE;
using Game.Battle.Core.Models;
using Game.Battle.Messaging.Clients;
using Game.Battle.Messaging.Mappers;
using Game.Battle.SignalR;
using Game.SharedKernel.Results;
using Microsoft.AspNetCore.SignalR;

namespace Game.Battle.Application.Battle;

public sealed class PveBattleService(
    BattleContext battleContext,
    IBattleRepository battleRepository,
    IGameBattleSnapshotClient gameBattleSnapshotClient,
    IHubContext<PveBattleHub, IPveBattleClient> hubContext,
    IPveBattleDomainEventProcessor domainEventProcessor) : IPveBattleService
{
    public async Task<Result<PveBattle>> StartBattle(string playerId, string monsterName, CancellationToken ct = default)
    {
        var snapshotResult = await gameBattleSnapshotClient.GetBattleSnapshotAsync(playerId, monsterName, ct);
        if (snapshotResult.IsFailure)
            return snapshotResult.AsError<PveBattle>();

        var playerResult = Result<Player>.Success(snapshotResult.Value.Player!.ToBattlePlayer());
        var monsterResult = Result<Monster>.Success(snapshotResult.Value.Monster!.ToBattleMonster());
        var battleResult = PveBattle.Create(playerResult, monsterResult);

        if (battleResult.IsFailure)
            return battleResult;

        var saveResult = await battleRepository.Save(battleResult.Value);
        return saveResult.IsFailure
            ? Result<PveBattle>.CustomError(saveResult.Error)
            : Result<PveBattle>.Success(battleResult.Value);
    }

    public Task<Result<PveBattle>> GetBattle(string battleId, CancellationToken ct = default) =>
        battleRepository.GetById(battleId, ct);

    public async Task<ResultWithoutValue> ExecutePlayerTurn(PveBattle battle, string abilityId, CancellationToken ct = default)
    {
        var executeResult = battle.ExecuteTurn(abilityId, battleContext);
        if (executeResult.IsFailure)
            return executeResult;

        await hubContext.Clients.Group(battle.Id).BattleData(new PveBattleViewModel(battle));

        var saveResult = await battleRepository.Save(battle);
        if (saveResult.IsFailure)
            return saveResult;

        await domainEventProcessor.Process(battle.DomainEvents, ct);
        battle.ResetDomainEvents();

        return ResultWithoutValue.Success();
    }
}
