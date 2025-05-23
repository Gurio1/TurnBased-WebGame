using Game.Core.Rewards;
using Game.Features.Battle.Contracts;

namespace Game.Features.Battle.PVE;

public interface IPveBattleClient
{
    Task BattleErrorMessage(string message);
    Task BattleData(PveBattleViewModel battle);
    Task BattleReward(BattleReward reward);
    Task BattleLose(bool isLose);
    Task Log(string reward);
}
