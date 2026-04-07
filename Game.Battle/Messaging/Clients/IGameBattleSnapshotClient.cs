using Game.SharedKernel.Battle;
using Game.SharedKernel.Contracts.Requests;
using Game.SharedKernel.Messaging;
using Game.SharedKernel.Results;

namespace Game.Battle.Messaging.Clients;

public interface IGameBattleSnapshotClient
{
    Task<Result<BattleStartSnapshotResponse>> GetBattleSnapshotAsync(string playerId, string monsterName, CancellationToken cancellationToken);
}
