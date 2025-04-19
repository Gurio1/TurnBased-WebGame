using Game.Core.Common;
using Game.Core.Models;
using Game.Features.Battle.Models;

namespace Game.Features.Battle;

public interface IBattleRepository
{
    Task<Result<PveBattle>> GetActiveBattleAsync(string battleId);
    Task<Result<PveBattle>> CreateBattleAsync(Player player);
    Task<ResultWithoutValue> SaveBattleData(PveBattle pveBattle);
    Task<bool> RemoveBattle(string battleId);
}
