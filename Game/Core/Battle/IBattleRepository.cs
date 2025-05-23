using Game.Application.SharedKernel;
using Game.Core.Battle.PVE;

namespace Game.Core.Battle;

public interface IBattleRepository
{
    Task<ResultWithoutValue> Delete(string battleId);
    //TODO : So actually i should not split battle to the PVE/PVP. It should be one battle,but in different context i handle it differently
    Task<ResultWithoutValue> Save(PveBattle battle);
}
