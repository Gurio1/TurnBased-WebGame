using Game.Battle.Core.Battle.PVE;
using Game.SharedKernel.Results;

namespace Game.Battle.Application.Battle;

public interface IPveBattleService
{
    Task<Result<PveBattle>> StartBattle(string playerId, string monsterName, CancellationToken ct = default);
    Task<Result<PveBattle>> GetBattle(string battleId, CancellationToken ct = default);
    Task<ResultWithoutValue> ExecutePlayerTurn(PveBattle battle, string abilityId, CancellationToken ct = default);
}
