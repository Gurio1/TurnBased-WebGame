using Game.Application.Battle;
using Game.Application.SharedKernel;
using Game.Core.Battle.PVE;
using Game.Features.Battle.Contracts;
using Game.Features.Battle.PVE.ExecutePlayerTurn;
using Game.Features.Battle.PVE.GetBattle;
using Game.Features.Battle.PVE.StartBattle;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Game.Features.Battle.PVE;

//As docs says TCP connections are limited per server.So for scale we need another server...To sync their connection we need to set up Redis backplane(For an unattainable future)
[Authorize]
public sealed class PveBattleHub : Hub<IPveBattleClient>
{
    private readonly IDispatcher dispatcher;
    private readonly IBattleAuthService authService;
    private readonly BattleCacheManager cacheManager;
    
    
    public PveBattleHub(IDispatcher dispatcher,IBattleAuthService authService,
        BattleCacheManager cacheManager)
    {
        this.dispatcher = dispatcher;
        this.authService = authService;
        this.cacheManager = cacheManager;
    }
    
    public override async Task OnConnectedAsync()
    {
        string? playerId = authService.GetCurrentPlayerId(Context.User);
        
        if (playerId is null)
        {
            await SendBattleError("User is not authenticated");
            return;
        }
        
        string? battleId = authService.TryGetBattleId(Context.User);
        
        var pveBattleResult = battleId is null
            ? await StartNewBattle(playerId)
            : await GetExistingBattle(battleId);
        
        if (pveBattleResult.IsFailure)
        {
            await SendBattleError(pveBattleResult.Error.Description);
            return;
        }
        
        var battle = pveBattleResult.Value;
        
        authService.AppendBattleIdToClaims(Context.User,battle.Id);
        
        await cacheManager.SetBattleIdCache(playerId, battle.Id);
        await ManageGroupMembership(battle.Id, join: true);
        await Clients.Group(battle.Id).BattleData(battle.ToViewModel());
        
        await base.OnConnectedAsync();
    }
    
    public async Task UseAbility(string abilityId)
    {
        var battle = await GetBattle();
        if (battle == null) return;
        
        await dispatcher.DispatchAsync(new ExecutePlayerTurnCommand(abilityId,battle));
    }
    
    public async Task SendBattleError(string message) => await Clients.Caller.BattleErrorMessage(message);
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string? playerId = authService.GetCurrentPlayerId(Context.User);
        if (playerId == null) return;
        //TODO : Handle situations when battleID is removed from cache when user was afk more than 30min
        string? battleId = await cacheManager.GetBattleId(playerId);
        if (battleId != null)
        {
            await ManageGroupMembership(battleId, join: false);
        }
        
        await cacheManager.Remove(playerId);
        await base.OnDisconnectedAsync(exception);
    }
    private async Task<PveBattle?> GetBattle()
    {
        string? playerId = authService.GetCurrentPlayerId(Context.User);
        if (playerId is null)
        {
            await SendBattleError("User is not authenticated");
            return null;
        }
        
        string? battleId = await cacheManager.GetBattleId(playerId);
        //TODO: Check for battle in mongo
        if (battleId is null)
        {
            await SendBattleError("Player is not in battle");
            return null;
        }
        
        var battleResult = await dispatcher.DispatchAsync(new GetBattleQuery(battleId));
        
        if (!battleResult.IsFailure) return battleResult.Value;
        
        await SendBattleError(battleResult.Error.Description);
        return null;
        
    }
    private async Task<Result<PveBattle>> StartNewBattle(string playerId)
        => await dispatcher.DispatchAsync(new StartBattleCommand("Goblin", playerId));
    
    private async Task<Result<PveBattle>> GetExistingBattle(string battleId)
        => await dispatcher.DispatchAsync(new GetBattleQuery(battleId));
    private async Task ManageGroupMembership(string battleId, bool join)
    {
        if (join)
            await Groups.AddToGroupAsync(Context.ConnectionId, battleId);
        else
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, battleId);
    }
}
