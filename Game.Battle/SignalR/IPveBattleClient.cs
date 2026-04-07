using Game.Battle.Contracts;
using Game.Battle.Core.Rewards;

namespace Game.Battle.SignalR;

public interface IPveBattleClient
{
    Task BattleErrorMessage(string message);
    Task BattleData(PveBattleViewModel battle);
    Task BattleLose(bool isLose);
    Task BattleReward(BattleReward reward);
    Task Log(string reward);
}
