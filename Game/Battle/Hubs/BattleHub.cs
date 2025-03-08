using Game.Core.Abilities;
using Game.Core.Models;
using Game.Core.Rewards;
using Microsoft.AspNetCore.SignalR;

namespace Game.Battle.Hubs;


//As docs says TCP connections are limited per server.So for scale we need another server...To sync their connection we need to set up Redis backplane(For an unattainable future)
internal class BattleHub : Hub
{
    private readonly BattleManager _battleManager;
    
    
    //In the future take playerId from JWT
    private Hero _character = new() {Abilities = [new BaseAttack(),new BleedAbility()]};

    public BattleHub(BattleManager battleManager)
    {
        _battleManager = battleManager;
    }
    
    public override async Task OnConnectedAsync()
    {
        var battle = await _battleManager.GetOrCreate(_character);
        await Clients.Caller.SendAsync("ReceiveBattleData", battle);
        await base.OnConnectedAsync();
    }

    public async Task UseAbility(int abilityId,string playerId)
    {
        _character.Id =  Guid.Parse(playerId);
        
        var battle = await _battleManager.GetOrCreate(_character);
        
        var result = await _battleManager.UseHeroAbility(abilityId,battle);

        if (result is not null)
        {
            await SendBattleReward(result);
        }
        
        await Clients.Caller.SendAsync("ReceiveBattleData", battle);
    }

    public async Task SendBattleReward(IReward reward)
    {
        await Clients.Caller.SendAsync("ReceiveBattleReward", reward);
    }
}