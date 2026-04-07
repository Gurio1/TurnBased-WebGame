using Game.SharedKernel.Battle;
using Game.SharedKernel.Contracts.Requests;
using Game.SharedKernel.Results;

namespace Game.Battle.Messaging.Clients;

public interface IBattleSettlementClient
{
    Task<Result<BattleResolveResponse>> ResolveBattleAsync(BattleResolveRequest request, CancellationToken ct);
}
