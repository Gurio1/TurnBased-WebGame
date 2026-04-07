using Game.Application.Monsters;
using Game.Application.Players;
using Game.SharedKernel.Contracts.Requests;
using Game.SharedKernel.Messaging;
using Game.Utilities.Extensions;
using MassTransit;

namespace Game.Messaging;

public sealed class BattleSnapshotConsumer(IPlayerService playerService, IMonsterService monsterService) : IConsumer<BattleStartSnapshotRequest>
{
    public async Task Consume(ConsumeContext<BattleStartSnapshotRequest> context)
    {
        var playerResult = await playerService.GetById(context.Message.PlayerId, context.CancellationToken);
        if (playerResult.IsFailure)
        {
            await context.RespondAsync(new BattleStartSnapshotResponse { ErrorMessage = playerResult.Error.Description });
            return;
        }

        var monsterResult = await monsterService.GetByName(context.Message.MonsterName, context.CancellationToken);
        if (monsterResult.IsFailure)
        {
            await context.RespondAsync(new BattleStartSnapshotResponse { ErrorMessage = monsterResult.Error.Description });
            return;
        }

        await context.RespondAsync(new BattleStartSnapshotResponse
        {
            Player = playerResult.Value.ToBattleSnapshot(),
            Monster = monsterResult.Value.ToBattleSnapshot()
        });
    }
}
