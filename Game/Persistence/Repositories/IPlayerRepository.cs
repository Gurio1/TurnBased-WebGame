using Game.Core.Models;
using Game.Core.SharedKernel;

namespace Game.Persistence.Repositories;

public interface IPlayerRepository
{
    Task<Result<Player>> GetById(string playerId, CancellationToken ct = default);
    Task<Result<Player>> GetByIdWithAbilities(string playerId, CancellationToken ct = default);
}
