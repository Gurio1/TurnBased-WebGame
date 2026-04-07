using Game.Battle.Application.Battle;
using Game.Battle.Contracts;
using Game.Battle.Core.Battle.PVE;
using Game.SharedKernel.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace Game.Battle.SignalR;

//As docs says TCP connections are limited per server.So for scale we need another server...To sync their connection we need to set up Redis backplane(For an unattainable future)
[Authorize]
public sealed class PveBattleHub : Hub<IPveBattleClient>
{
    private readonly IPveBattleService pveBattleService;
    private readonly IBattleUserContext battleUserContext;
    private readonly PlayerBattleCache playerBattleCache;
    
    
    public PveBattleHub(IPveBattleService pveBattleService,IBattleUserContext battleUserContext,
        PlayerBattleCache playerBattleCache)
    {
        this.pveBattleService = pveBattleService;
        this.battleUserContext = battleUserContext;
        this.playerBattleCache = playerBattleCache;
    }
    
    public override async Task OnConnectedAsync()
    {
        string? playerId = battleUserContext.GetCurrentPlayerId(Context.User);
        
        if (playerId is null)
        {
            await SendBattleError("User is not authenticated");
            return;
        }
        
        string? battleId = await playerBattleCache.GetBattleId(playerId) ?? battleUserContext.TryGetBattleId(Context.User);
        
        var pveBattleResult = battleId is null
            ? await StartNewBattle(playerId)
            : await GetExistingBattle(battleId);
        
        if (pveBattleResult.IsFailure)
        {
            await SendBattleError(pveBattleResult.Error.Description);
            return;
        }
        
        var battle = pveBattleResult.Value;
        
        battleUserContext.AppendBattleIdToClaims(Context.User,battle.Id);
        
        await playerBattleCache.SetBattleIdCache(playerId, battle.Id);
        await ManageGroupMembership(battle.Id, join: true);
        await Clients.Group(battle.Id).BattleData(new PveBattleViewModel(battle));
        
        await base.OnConnectedAsync();
    }
    
    public async Task UseAbility(string abilityId)
    {
        var battle = await GetBattle();
        if (battle == null) return;
        
        await pveBattleService.ExecutePlayerTurn(battle, abilityId);
    }
    
    public async Task SendBattleError(string message) => await Clients.Caller.BattleErrorMessage(message);
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string? playerId = battleUserContext.GetCurrentPlayerId(Context.User);
        if (playerId == null) return;
        //TODO : Handle situations when battleID is removed from cache when user was afk more than 30min
        string? battleId = await playerBattleCache.GetBattleId(playerId);
        if (battleId != null)
        {
            await ManageGroupMembership(battleId, join: false);
        }
        
        await playerBattleCache.Remove(playerId);
        await base.OnDisconnectedAsync(exception);
    }
    private async Task<PveBattle?> GetBattle()
    {
        string? playerId = battleUserContext.GetCurrentPlayerId(Context.User);
        if (playerId is null)
        {
            await SendBattleError("User is not authenticated");
            return null;
        }
        
        string? battleId = await playerBattleCache.GetBattleId(playerId);
        //TODO: Check for battle in mongo
        if (battleId is null)
        {
            await SendBattleError("Player is not in battle");
            return null;
        }
        
        var battleResult = await pveBattleService.GetBattle(battleId);
        
        if (!battleResult.IsFailure) return battleResult.Value;
        
        await SendBattleError(battleResult.Error.Description);
        return null;
        
    }
    private async Task<Result<PveBattle>> StartNewBattle(string playerId)
        => await pveBattleService.StartBattle(playerId, "Goblin");
    
    private async Task<Result<PveBattle>> GetExistingBattle(string battleId)
        => await pveBattleService.GetBattle(battleId);
    private async Task ManageGroupMembership(string battleId, bool join)
    {
        if (join)
            await Groups.AddToGroupAsync(Context.ConnectionId, battleId);
        else
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, battleId);
    }
}
