using System.Security.Claims;
using Game.Core.SharedKernel;
using Game.Features.Battle.Models;
using Game.Features.Battle.PVE.GetBattle;
using Game.Features.Battle.PVE.StartBattle;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;

namespace Game.Features.Battle.PVE;

//As docs says TCP connections are limited per server.So for scale we need another server...To sync their connection we need to set up Redis backplane(For an unattainable future)
[Authorize]
public sealed class PveBattleHub : Hub
{
    private const string PlayerIdClaim = "PlayerId";
    private const string BattleIdClaim = "BattleId";
    private const string BattleCachePrefix = "battle:";
    private readonly IDistributedCache cache;
    private readonly IDispatcher dispatcher;
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly PveBattleManager pveBattleManager;
    
    
    public PveBattleHub(PveBattleManager pveBattleManager, IDispatcher dispatcher,
        IDistributedCache cache, IHttpContextAccessor httpContextAccessor)
    {
        this.pveBattleManager = pveBattleManager;
        this.dispatcher = dispatcher;
        this.cache = cache;
        this.httpContextAccessor = httpContextAccessor;
    }
    
    public override async Task OnConnectedAsync()
    {
        string? playerId = GetCurrentPlayerId();
        
        if (playerId is null)
        {
            await SendBattleError("User is not authenticated");
            return;
        }
        
        string? battleId = TryGetBattleId();
        
        Result<PveBattle> pveBattleResult;
        
        if (battleId is null)
            pveBattleResult = await dispatcher.DispatchAsync(new StartBattleCommand("Goblin", playerId));
        else
            pveBattleResult = await dispatcher.DispatchAsync(new GetBattleQuery(battleId));
        
        if (pveBattleResult.IsFailure)
        {
            await SendBattleError(pveBattleResult.Error.Description);
            return;
        }
        
        var battle = pveBattleResult.Value;
        
        AppendBattleIdToClaims(battle.Id);
        
        try
        {
            string cacheKey = GetBattleCacheKey(playerId);
            await cache.SetStringAsync(cacheKey, battle.Id,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) });
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
        
        await Groups.AddToGroupAsync(Context.ConnectionId, battle.Id);
        await Clients.Group(battle.Id).SendAsync("ReceiveBattleData", battle);
        
        await base.OnConnectedAsync();
    }
    
    public async Task UseAbility(string abilityId)
    {
        string? playerId = GetCurrentPlayerId();
        
        if (playerId is null)
        {
            await SendBattleError("User is not authenticated");
            return;
        }
        
        string cacheKey = GetBattleCacheKey(playerId);
        string? battleId = await cache.GetStringAsync(cacheKey);
        
        if (battleId is null)
        {
            await SendBattleError("Can not use ability. Player is not in battle");
            return;
        }
        
        var battleResult = await dispatcher.DispatchAsync(new GetBattleQuery(battleId));
        
        if (battleResult.IsFailure)
        {
            await SendBattleError(battleResult.Error.Description);
            return;
        }
        
        await pveBattleManager.ExecutePlayerTurnAsync(abilityId, battleResult.Value);
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string? playerId = GetCurrentPlayerId();
        
        if (playerId != null)
        {
            string cacheKey = GetBattleCacheKey(playerId);
            
            //TODO : Handle situations when battleID is removed from cache when user was afk more than 30min
            string? battleId = await cache.GetStringAsync(cacheKey);
            
            await Groups.RemoveFromGroupAsync(playerId, battleId);
            
            await cache.RemoveAsync($"battle:{playerId}");
        }
        
        await base.OnDisconnectedAsync(exception);
    }
    
    private string? GetCurrentPlayerId() => Context.User?.FindFirstValue(PlayerIdClaim);
    private string? TryGetBattleId() => Context.User?.FindFirstValue(BattleIdClaim);
    
    private void AppendBattleIdToClaims(string battleId)
    {
        var currentUser = httpContextAccessor.HttpContext!.User;
        
        if (currentUser.HasClaim(c => c.Type == BattleIdClaim))
            return;
        
        var identity = currentUser.Identity as ClaimsIdentity;
        identity?.AddClaim(new Claim("BattleId", battleId));
        
        var principal = new ClaimsPrincipal(identity!);
        httpContextAccessor.HttpContext.SignInAsync(principal);
    }
    
    private static string GetBattleCacheKey(string playerId) => $"{BattleCachePrefix}{playerId}";
    
    public async Task SendBattleError(string message) =>
        await Clients.Caller.SendAsync("ReceiveBattleErrorMessage", message);
}
