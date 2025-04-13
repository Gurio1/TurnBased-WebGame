using Game.Logger;
using MediatR;

namespace Game.Features.Battle.Models;

public class BattleContext
{
    private readonly IMediator mediator;
    private string battleId = string.Empty;

    public BattleContext(IMediator mediator) => this.mediator = mediator;
    
    public void PublishActionLog(string message) => mediator.Publish(new ActionLogNotification(battleId,message));
    
    public string GetBattleId()
    {
        if (battleId == string.Empty)
        {
            throw new InvalidDataException("Battle id is not set");
        }

        return battleId;
    }

    public void SetBattleId(string id) => battleId = id;
}
