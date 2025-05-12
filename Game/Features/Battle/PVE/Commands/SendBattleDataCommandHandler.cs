using Game.Core.SharedKernel;
using Game.Features.Battle.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Game.Features.Battle.PVE.Commands;

public class SendBattleDataCommandHandler : IRequestHandler<SendBattleDataCommand, ResultWithoutValue>
{
    private readonly IHubContext<PveBattleHub> hubContext;
    
    public SendBattleDataCommandHandler(IHubContext<PveBattleHub> hubContext) => this.hubContext = hubContext;
    
    public async Task<ResultWithoutValue> Handle(SendBattleDataCommand notification,
        CancellationToken cancellationToken)
    {
        await hubContext.Clients.Group(notification.Battle.Id)
            .SendAsync("ReceiveBattleData", notification.Battle.ToViewModel(), cancellationToken);
        
        return ResultWithoutValue.Success();
    }
}
