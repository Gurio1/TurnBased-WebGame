using Game.SharedKernel.Battle;
using Game.SharedKernel.Contracts.Requests;
using Game.SharedKernel.Results;
using MassTransit;

namespace Game.Battle.Messaging.Clients;

public sealed class BattleSettlementClient(IRequestClient<BattleResolveRequest> requestClient) : IBattleSettlementClient
{
    private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(10);

    public async Task<Result<BattleResolveResponse>> ResolveBattleAsync(BattleResolveRequest request, CancellationToken ct)
    {
        using var timeoutCts = new CancellationTokenSource(RequestTimeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(ct, timeoutCts.Token);

        try
        {
            var response = await requestClient.GetResponse<BattleResolveResponse>(request, linkedCts.Token);
            return string.IsNullOrWhiteSpace(response.Message.ErrorMessage)
                ? Result<BattleResolveResponse>.Success(response.Message)
                : Result<BattleResolveResponse>.Failure(response.Message.ErrorMessage);
        }
        catch (OperationCanceledException) when (!ct.IsCancellationRequested)
        {
            return Result<BattleResolveResponse>.Failure("Battle settlement timed out.");
        }
        catch (Exception ex)
        {
            return Result<BattleResolveResponse>.Failure(ex.Message);
        }
    }
}
