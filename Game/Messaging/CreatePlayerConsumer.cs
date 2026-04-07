using Game.Application.Players;
using Game.SharedKernel.Contracts.Requests;
using Game.SharedKernel.Messaging;
using MassTransit;

namespace Game.Messaging;

public sealed class CreatePlayerConsumer(IPlayerService playerService) : IConsumer<CreatePlayerRequest>
{
    public async Task Consume(ConsumeContext<CreatePlayerRequest> context)
    {
        var result = await playerService.Create(context.CancellationToken);

        await context.RespondAsync(result.IsSuccess
            ? new CreatePlayerResponse { PlayerId = result.Value }
            : new CreatePlayerResponse { ErrorMessage = result.Error.Description });
    }
}
