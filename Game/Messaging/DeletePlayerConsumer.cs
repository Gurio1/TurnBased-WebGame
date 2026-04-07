using Game.Application.Players;
using Game.SharedKernel.Contracts.Requests;
using Game.SharedKernel.Messaging;
using MassTransit;

namespace Game.Messaging;

public sealed class DeletePlayerConsumer(IPlayerService playerService) : IConsumer<DeletePlayerRequest>
{
    public async Task Consume(ConsumeContext<DeletePlayerRequest> context)
    {
        var result = await playerService.Delete(context.Message.PlayerId, context.CancellationToken);

        await context.RespondAsync(result.IsSuccess
            ? new DeletePlayerResponse { Success = true }
            : new DeletePlayerResponse { ErrorMessage = result.Error.Description });
    }
}
