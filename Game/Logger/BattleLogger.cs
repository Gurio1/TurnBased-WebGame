using Game.Features.Battle.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Game.Logger;

public class BattleLogger : INotificationHandler<ActionLogNotification>
{
    private readonly IHubContext<BattleHub> battleHubContext;

    public BattleLogger(IHubContext<BattleHub> battleHubContext) => this.battleHubContext = battleHubContext;
    
    public async Task Handle(ActionLogNotification notification, CancellationToken cancellationToken) =>
        await battleHubContext.Clients.Group(notification.BattleId)
            .SendAsync("Log",new {Message = notification.ActionLog}, cancellationToken: cancellationToken);
}
