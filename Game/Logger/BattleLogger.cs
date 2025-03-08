using Game.Battle.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Game.Logger;

internal class BattleLogger : INotificationHandler<ActionLogNotification>
{
    private readonly IHubContext<BattleHub> _battleHubContext;

    public BattleLogger(IHubContext<BattleHub> battleHubContext)
    {
        _battleHubContext = battleHubContext;
    }
    public async Task Handle(ActionLogNotification notification, CancellationToken cancellationToken)
    {
       await _battleHubContext.Clients.All.SendAsync("Log",new {Message = notification.ActionLog}, cancellationToken: cancellationToken);
    }
}