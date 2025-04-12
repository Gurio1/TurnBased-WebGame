using MediatR;

namespace Game.Logger;

public class ActionLogNotification(string battleId,string message) : INotification
{
    public string BattleId { get; set; } = battleId;
    public string ActionLog { get; set; } = message;
}