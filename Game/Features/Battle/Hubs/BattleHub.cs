using Game.Core.Rewards;
using Game.Features.Abilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Game.Features.Battle.Hubs;


//As docs says TCP connections are limited per server.So for scale we need another server...To sync their connection we need to set up Redis backplane(For an unattainable future)

[Authorize]
internal class BattleHub : Hub
{
    private readonly BattleManager _battleManager;
    private readonly BattleService _battleService;
    private string _playerId;
    public BattleHub(BattleManager battleManager,BattleService battleService)
    {
        _battleManager = battleManager;
        _battleService = battleService;
    }
    
    public override async Task OnConnectedAsync()
    {
        _playerId = Context.User.Claims.SingleOrDefault(x => x.Type == "PlayerId").Value;
        

        var battle =  await _battleService.GetOrCreate(_playerId);
        
        await Groups.AddToGroupAsync(Context.ConnectionId, battle.Id);
        await Clients.Group(battle.Id).SendAsync("ReceiveBattleData", battle);
        await base.OnConnectedAsync();
    }

    public async Task UseAbility(string abilityId)
    {
        
        var battle = await _battleService.GetOrCreate(Context.User.Claims.SingleOrDefault(x => x.Type == "PlayerId").Value);
        
        var result = await _battleManager.UseHeroAbility(abilityId,battle);

        if (result is not null)
        {
            await SendBattleReward(result);
        }
        
        await Clients.Group(battle.Id).SendAsync("ReceiveBattleData", battle);
    }

    public async Task SendBattleReward(IReward reward)
    {
        await Clients.Caller.SendAsync("ReceiveBattleReward", reward);
    }
}