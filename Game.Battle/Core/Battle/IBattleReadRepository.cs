using Game.Battle.Core.Battle.PVE;
using Game.SharedKernel.Results;

namespace Game.Battle.Core.Battle;

public interface IBattleReadRepository
{
    Task<Result<PveBattle>> GetById(string battleId, CancellationToken ct = default);
}
