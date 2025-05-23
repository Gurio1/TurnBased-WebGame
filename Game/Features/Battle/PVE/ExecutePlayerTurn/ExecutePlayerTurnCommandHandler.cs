using Game.Application.SharedKernel;
using Game.Core.Battle;
using Game.Features.Battle.Contracts;
using Microsoft.AspNetCore.SignalR;

namespace Game.Features.Battle.PVE.ExecutePlayerTurn;

public sealed class ExecutePlayerTurnCommandHandler : IRequestHandler<ExecutePlayerTurnCommand, ResultWithoutValue>
{
    private readonly BattleContext battleContext;
    private readonly IBattleRepository battleRepository;
    private readonly IHubContext<PveBattleHub, IPveBattleClient> hubContext;
    private readonly IDispatcher dispatcher;
    
    
    public ExecutePlayerTurnCommandHandler(BattleContext battleContext,IBattleRepository battleRepository,
        IHubContext<PveBattleHub, IPveBattleClient> hubContext, IDispatcher dispatcher)
    {
        this.battleContext = battleContext;
        this.battleRepository = battleRepository;
        this.hubContext = hubContext;
        this.dispatcher = dispatcher;
    }
    public async Task<ResultWithoutValue> Handle(ExecutePlayerTurnCommand request, CancellationToken cancellationToken)
    {
        request.PveBattle.ExecuteTurn(request.AbilityId, battleContext);
        
        await hubContext.Clients.Group(request.PveBattle.Id).BattleData(request.PveBattle.ToViewModel());
        
        var saveResult = await battleRepository.Save(request.PveBattle);
        
        foreach (var @event in request.PveBattle.DomainEvents)
        {
            await dispatcher.PublishAsync(@event, cancellationToken);
        }
        
        request.PveBattle.ResetDomainEvents();
        
        return saveResult.IsFailure
            ? ResultWithoutValue.CreateError(saveResult.Error)
            : ResultWithoutValue.Success();
    }
}
