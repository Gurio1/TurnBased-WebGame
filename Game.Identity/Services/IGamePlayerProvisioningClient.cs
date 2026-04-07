using Game.SharedKernel.Messaging;
using Game.SharedKernel.Results;

namespace Game.Identity.Services;

public interface IGamePlayerProvisioningClient
{
    Task<Result<CreatePlayerResponse>> CreatePlayerAsync(CancellationToken cancellationToken);
    Task<Result<DeletePlayerResponse>> DeletePlayerAsync(string playerId, CancellationToken cancellationToken);
}
