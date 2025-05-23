using Game.Application.SharedKernel;

namespace Game.Core.PlayerProfile;

public interface IPlayerRepository
{
    Task<Result<Player>> GetById(string playerId, CancellationToken ct = default);
    Task<Result<Player>> GetByIdWithAbilities(string playerId, CancellationToken ct = default);
}
