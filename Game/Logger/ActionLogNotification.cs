using MediatR;

namespace Game.Logger;

internal class ActionLogNotification(string message) : INotification
{
    public string ActionLog { get; set; } = message;
}