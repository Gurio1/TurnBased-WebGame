using Game.Features.Battle.PVE;
using Microsoft.AspNetCore.SignalR;

namespace Game.Core.Battle;

public class BattleContext
{
    private readonly IHubContext<PveBattleHub, IPveBattleClient> hubContext;
    private string battleId = string.Empty;
    
    public BattleContext(IHubContext<PveBattleHub, IPveBattleClient> hubContext) => this.hubContext = hubContext;
    
    public void PublishActionLog(string message) =>
        hubContext.Clients.Group(battleId).Log(message);
    
    public string GetBattleId()
    {
        if (battleId == string.Empty) throw new InvalidDataException("Battle id is not set");
        
        return battleId;
    }
    
    public void SetBattleId(string id) => battleId = id;
}
