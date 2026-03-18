using Game.Core.PlayerProfile.Aggregates;
using Game.SharedKernel;

namespace Game.Core.PlayerProfile;

public interface IPlayerRepository
{
    Task<Result<GamePlayer>> GetById(string playerId, CancellationToken ct = default);
    Task<Result<GamePlayer>> GetByIdWithAbilities(string playerId, CancellationToken ct = default);
}
