using Game.Features.Battle.Contracts;
using Game.Features.Battle.Hubs;
using Game.Features.Battle.PVE.Events;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Game.Features.Battle.PVE.Handlers;

public class PveBattleDataSentEventHandler : INotificationHandler<PveBattleDataSentEvent>
{
    private readonly IHubContext<BattleHub> _hubContext;

    public PveBattleDataSentEventHandler(IHubContext<BattleHub> hubContext)
    {
        _hubContext = hubContext;
    }
    public async Task Handle(PveBattleDataSentEvent notification, CancellationToken cancellationToken)
    {
        await _hubContext.Clients.Group(notification.Battle.Id).
            SendAsync("ReceiveBattleData", notification.Battle.ToViewModel(), cancellationToken: cancellationToken);
    }
}