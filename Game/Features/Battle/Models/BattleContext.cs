using Game.Logger;
using MediatR;

namespace Game.Features.Battle.Models;

public class BattleContext
{
    private readonly IMediator _mediator;
    private string _battleId = string.Empty;

    public BattleContext(IMediator mediator)
    {
        _mediator = mediator;
    }

    public void PublishActionLog(string message)
    {
        _mediator.Publish(new ActionLogNotification(_battleId,message));
    }

    public string GetBattleId()
    {
        if (_battleId == string.Empty)
        {
            throw new Exception("Battle id is not set");
        }

        return _battleId;
    }

    public void SetBattleId(string battleId)
    {
        _battleId = battleId;
    }
}