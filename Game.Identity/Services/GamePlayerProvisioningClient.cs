using MassTransit;
using Game.SharedKernel.Contracts.Requests;
using Game.SharedKernel.Messaging;
using Game.SharedKernel.Results;

namespace Game.Identity.Services;

public sealed class GamePlayerProvisioningClient(
    IRequestClient<CreatePlayerRequest> createPlayerClient,
    IRequestClient<DeletePlayerRequest> deletePlayerClient) : IGamePlayerProvisioningClient
{
    private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(10);

    public async Task<Result<CreatePlayerResponse>> CreatePlayerAsync(CancellationToken cancellationToken)
    {
        var result = await GetResponseAsync<CreatePlayerRequest, CreatePlayerResponse>(
            createPlayerClient,
            new CreatePlayerRequest(),
            cancellationToken);

        if (result.IsFailure)
            return result;

        return string.IsNullOrWhiteSpace(result.Value.PlayerId)
            ? Result<CreatePlayerResponse>.Failure("Game service returned an empty player id.")
            : result;
    }

    public async Task<Result<DeletePlayerResponse>> DeletePlayerAsync(string playerId, CancellationToken cancellationToken)
    {
        var result = await GetResponseAsync<DeletePlayerRequest, DeletePlayerResponse>(
            deletePlayerClient,
            new DeletePlayerRequest { PlayerId = playerId },
            cancellationToken);

        if (result.IsFailure)
            return result;

        return result.Value.Success
            ? result
            : Result<DeletePlayerResponse>.Failure("Game service rejected player cleanup.");
    }

    private static async Task<Result<TResponse>> GetResponseAsync<TRequest, TResponse>(
        IRequestClient<TRequest> client,
        TRequest request,
        CancellationToken cancellationToken)
        where TRequest : class
        where TResponse : class
    {
        using var timeoutCts = new CancellationTokenSource(RequestTimeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        try
        {
            var response = await client.GetResponse<TResponse>(request, linkedCts.Token);
            return Result<TResponse>.Success(response.Message);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return Result<TResponse>.Failure("Request to Game service timed out.");
        }
        catch (Exception ex)
        {
            return Result<TResponse>.Failure(ex.Message);
        }
    }
}
