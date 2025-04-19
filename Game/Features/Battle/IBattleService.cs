using Game.Core;
using Game.Core.Common;
using Game.Core.Models;
using Game.Features.Battle.Models;

namespace Game.Features.Battle;

public interface IBattleService
{
    Task<Result<PveBattle>> InitializeBattleForPlayerAsync(string playerId);
    Task<Result<PveBattle>> GetActiveBattleAsync(string playerId);
}