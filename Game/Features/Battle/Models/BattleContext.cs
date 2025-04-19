using Game.Core.Common;

namespace Game.Features.Battle.Models;

public class BattleContext
{
    private readonly IDispatcher dispatcher;
    private string battleId = string.Empty;
    
    public BattleContext(IDispatcher dispatcher) => this.dispatcher = dispatcher;
    
    public void PublishActionLog(string message) => dispatcher.Dispatch(new SendActionLogCommand(battleId, message));
    
    public string GetBattleId()
    {
        if (battleId == string.Empty) throw new InvalidDataException("Battle id is not set");
        
        return battleId;
    }
    
    public void SetBattleId(string id) => battleId = id;
}
