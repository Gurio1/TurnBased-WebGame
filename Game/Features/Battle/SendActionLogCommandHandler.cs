using Game.Application.Features.Battle.PVE;
using Game.Core.SharedKernel;
using Microsoft.AspNetCore.SignalR;

namespace Game.Features.Battle;

public class SendActionLogCommandHandler : IRequestHandler<SendActionLogCommand, ResultWithoutValue>
{
    private readonly IHubContext<PveBattleHub> battleHubContext;
    
    public SendActionLogCommandHandler(IHubContext<PveBattleHub> battleHubContext) =>
        this.battleHubContext = battleHubContext;
    
    public async Task<ResultWithoutValue> Handle(SendActionLogCommand request, CancellationToken cancellationToken)
    {
        await battleHubContext.Clients.Group(request.BattleId)
            .SendAsync("Log", new { request.Message }, cancellationToken);
        
        return ResultWithoutValue.Success();
    }
}
