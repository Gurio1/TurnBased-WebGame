using Game.Features.Battle.Contracts;
using Game.Features.Battle.Hubs;
using MediatR;
using Microsoft.AspNetCore.SignalR;

namespace Game.Features.Battle.PVE.Events;

public class PveBattleDataSentHandler : INotificationHandler<PveBattleDataSentEvent>
{
    private readonly IHubContext<BattleHub> hubContext;

    public PveBattleDataSentHandler(IHubContext<BattleHub> hubContext) => this.hubContext = hubContext;
    
    public async Task Handle(PveBattleDataSentEvent notification, CancellationToken cancellationToken) =>
        await hubContext.Clients.Group(notification.Battle.Id).
            SendAsync("ReceiveBattleData", notification.Battle.ToViewModel(), cancellationToken: cancellationToken);
}
