using Game.Application.Players;
using Game.SharedKernel.Battle;
using Game.SharedKernel.Contracts.Requests;
using MassTransit;

namespace Game.Messaging;

public sealed class BattleSettlementConsumer(IPlayerService playerService) : IConsumer<BattleResolveRequest>
{
    public async Task Consume(ConsumeContext<BattleResolveRequest> context)
    {
        var result = await playerService.ResolveBattle(
            context.Message.PlayerId,
            context.Message,
            context.CancellationToken);

        await context.RespondAsync(result.IsFailure
            ? new BattleResolveResponse { ErrorMessage = result.Error.Description }
            : result.Value);
    }
}

