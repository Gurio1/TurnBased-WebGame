using Game.Core.Common;
using Game.Features.Battle.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Game.Features.Battle;

public class SendActionLogCommandHandler : IRequestHandler<SendActionLogCommand, ResultWithoutValue>
{
    private readonly IHubContext<BattleHub> battleHubContext;
    
    public SendActionLogCommandHandler(IHubContext<BattleHub> battleHubContext) =>
        this.battleHubContext = battleHubContext;
    
    public async Task<ResultWithoutValue> Handle(SendActionLogCommand request, CancellationToken cancellationToken)
    {
        await battleHubContext.Clients.Group(request.BattleId)
            .SendAsync("Log", new { request.Message }, cancellationToken);
        
        return ResultWithoutValue.Success();
    }
}
