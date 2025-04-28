using Game.Core.SharedKernel;
using Game.Features.Battle.Models;

namespace Game.Features.Battle;

public interface IBattleService
{
    Task<Result<PveBattle>> InitializeBattleForPlayerAsync(string playerId);
    Task<Result<PveBattle>> GetActiveBattleAsync(string playerId);
}
