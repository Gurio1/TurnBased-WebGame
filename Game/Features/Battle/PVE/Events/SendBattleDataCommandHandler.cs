using Game.Core.Common;
using Game.Features.Battle.Contracts;
using Game.Features.Battle.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Game.Features.Battle.PVE.Events;

public class SendBattleDataCommandHandler : IRequestHandler<SendBattleDataCommand, ResultWithoutValue>
{
    private readonly IHubContext<BattleHub> hubContext;
    
    public SendBattleDataCommandHandler(IHubContext<BattleHub> hubContext) => this.hubContext = hubContext;
    
    public async Task<ResultWithoutValue> Handle(SendBattleDataCommand notification,
        CancellationToken cancellationToken)
    {
        await hubContext.Clients.Group(notification.Battle.Id)
            .SendAsync("ReceiveBattleData", notification.Battle.ToViewModel(), cancellationToken);
        
        return ResultWithoutValue.Success();
    }
}
