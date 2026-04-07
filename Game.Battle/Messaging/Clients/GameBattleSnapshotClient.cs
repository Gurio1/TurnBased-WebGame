using Game.SharedKernel.Battle;
using Game.SharedKernel.Contracts.Requests;
using Game.SharedKernel.Messaging;
using Game.SharedKernel.Results;
using MassTransit;

namespace Game.Battle.Messaging.Clients;

public sealed class GameBattleSnapshotClient(IRequestClient<BattleStartSnapshotRequest> requestClient)
    : IGameBattleSnapshotClient
{
    private static readonly TimeSpan RequestTimeout = TimeSpan.FromSeconds(10);

    public async Task<Result<BattleStartSnapshotResponse>> GetBattleSnapshotAsync(string playerId, string monsterName, CancellationToken cancellationToken)
    {
        using var timeoutCts = new CancellationTokenSource(RequestTimeout);
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, timeoutCts.Token);

        try
        {
            var response = await requestClient.GetResponse<BattleStartSnapshotResponse>(
                new BattleStartSnapshotRequest
                {
                    PlayerId = playerId,
                    MonsterName = monsterName
                },
                linkedCts.Token);

            if (!string.IsNullOrWhiteSpace(response.Message.ErrorMessage))
            {
                return Result<BattleStartSnapshotResponse>.Failure(response.Message.ErrorMessage);
            }

            return response.Message.Player is null || response.Message.Monster is null
                ? Result<BattleStartSnapshotResponse>.Failure("Battle snapshot could not be assembled.")
                : Result<BattleStartSnapshotResponse>.Success(response.Message);
        }
        catch (OperationCanceledException) when (!cancellationToken.IsCancellationRequested)
        {
            return Result<BattleStartSnapshotResponse>.Failure("Request to Game service timed out.");
        }
        catch (Exception ex)
        {
            return Result<BattleStartSnapshotResponse>.Failure(ex.Message);
        }
    }
}
