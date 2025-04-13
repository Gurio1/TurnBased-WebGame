using System.Security.Claims;
using Game.Core;
using Game.Features.Battle.Models;
using Game.Features.Players;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;

namespace Game.Features.Battle.Hubs;


//As docs says TCP connections are limited per server.So for scale we need another server...To sync their connection we need to set up Redis backplane(For an unattainable future)

[Authorize]
public class BattleHub : Hub
{
    private const string PlayerIdClaim = "PlayerId";
    private const string BattleCachePrefix = "battle:";
    
    private readonly PveBattleManager pveBattleManager;
    private readonly IBattleService battleRedisService;
    private readonly IDistributedCache cache;
    

    public BattleHub(PveBattleManager pveBattleManager,IBattleService battleRedisService,
         IDistributedCache cache)
    {
        this.pveBattleManager = pveBattleManager;
        this.battleRedisService = battleRedisService;
        this.cache = cache;
    }
    
    public override async Task OnConnectedAsync()
    {
        string? playerId = GetCurrentPlayerId();

        if (playerId is null)
        {
            await SendBattleError("User is not authenticated");
            return;
        }
        
        var battleResult = await battleRedisService.InitializeBattleForPlayerAsync(playerId);

        if (battleResult.IsFailure)
        {
            await SendBattleError(((ResultWithoutValue)battleResult).Error.Description);
            return;
        }

        var battle = battleResult.Value;

        try
        {
            string cacheKey = GetBattleCacheKey(playerId);
            await cache.SetStringAsync(cacheKey, battle.Id, new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30)
            });
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
        
        var battleResult = await battleRedisService.GetActiveBattleAsync(battleId);
        if (battleResult.IsFailure)
        {
            await SendBattleError(((ResultWithoutValue)battleResult).Error.Description);
            return;
        }
        
        await pveBattleManager.ExecutePlayerTurnAsync(abilityId,battleResult.Value);
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        string? playerId= GetCurrentPlayerId();
        
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
    
    private static string GetBattleCacheKey(string playerId) => $"{BattleCachePrefix}{playerId}";
    
    public async Task SendBattleError(string message) =>
        await Clients.Caller.SendAsync("ReceiveBattleErrorMessage",message);
}
