using Game.Features.Battle.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Game.Logger;

public class BattleLogger : INotificationHandler<ActionLogNotification>
{
    private readonly IHubContext<BattleHub> _battleHubContext;

    public BattleLogger(IHubContext<BattleHub> battleHubContext)
    {
        _battleHubContext = battleHubContext;
    }
    public async Task Handle(ActionLogNotification notification, CancellationToken cancellationToken)
    {
       await _battleHubContext.Clients.Group(notification.BattleId).SendAsync("Log",new {Message = notification.ActionLog}, cancellationToken: cancellationToken);
    }
}