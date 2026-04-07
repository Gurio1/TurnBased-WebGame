using Game.Battle.Core.Battle.PVE;
using Game.SharedKernel.Results;

namespace Game.Battle.Core.Battle;

public interface IBattleRepository
{
    Task<Result<PveBattle>> GetById(string battleId, CancellationToken ct = default);
    Task<ResultWithoutValue> Delete(string battleId);
    //TODO : So actually i should not split battle to the PVE/PVP. It should be one battle,but in different context i handle it differently
    Task<ResultWithoutValue> Save(PveBattle battle);
}
