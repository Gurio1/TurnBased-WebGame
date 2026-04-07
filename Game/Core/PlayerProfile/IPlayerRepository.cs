using Game.Core.PlayerProfile.Aggregates;
using Game.SharedKernel;
using Game.SharedKernel.Results;

namespace Game.Core.PlayerProfile;

public interface IPlayerRepository
{
    Task<Result<GamePlayer>> GetById(string playerId, CancellationToken ct = default);
    Task<Result<GamePlayer>> Create(GamePlayer player, CancellationToken ct = default);
    Task<Result<GamePlayer>> Save(GamePlayer player, CancellationToken ct = default);
    Task<ResultWithoutValue> Delete(string playerId, CancellationToken ct = default);
}
