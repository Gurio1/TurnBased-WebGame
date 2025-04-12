using System.Security.Claims;
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
    
    private readonly PveBattleManager _pveBattleManager;
    private readonly IBattleService _battleRedisService;
    private readonly IDistributedCache _cache;
    

    public BattleHub(PveBattleManager pveBattleManager,IBattleService battleRedisService,
         IDistributedCache cache)
    {
        _pveBattleManager = pveBattleManager;
        _battleRedisService = battleRedisService;
        _cache = cache;
    }
    
    public override async Task OnConnectedAsync()
    {
        var playerId = GetCurrentPlayerId();

        if (playerId is null)
        {
            await SendBattleError("User is not authenticated");
            return;
        }
        
        var battleResult = await _battleRedisService.InitializeBattleForPlayerAsync(playerId);

        if (battleResult.IsFailure)
        {
            await SendBattleError(battleResult.Error.Description);
            return;
        }

        var battle = battleResult.Value;

        try
        {
            var cacheKey = GetBattleCacheKey(playerId);
            await _cache.SetStringAsync(cacheKey, battle.Id, new DistributedCacheEntryOptions
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
        var playerId = GetCurrentPlayerId();

        if (playerId is null)
        {
            await SendBattleError("User is not authenticated");
            return;
        }        
        
        var cacheKey = GetBattleCacheKey(playerId);
        var battleId = await _cache.GetStringAsync(cacheKey);

        if (battleId is null)
        {
            await SendBattleError("Can not use ability. Player is not in battle");
            return;
        }
        
        var battleResult = await _battleRedisService.GetActiveBattleAsync(battleId);
        if (battleResult.IsFailure)
        {
            await SendBattleError(battleResult.Error.Description);
            return;
        }
        
        await _pveBattleManager.ExecutePlayerTurnAsync(abilityId,battleResult.Value);
    }
    
    public override async Task OnDisconnectedAsync(Exception? exception)
    {
        var playerId= GetCurrentPlayerId();
        
        if (playerId != null)
        {
            var cacheKey = GetBattleCacheKey(playerId);
            
            //TODO : Handle situations when battleID is removed from cache when user was afk more than 30min
            var battleId = await _cache.GetStringAsync(cacheKey);
        
            await Groups.RemoveFromGroupAsync(playerId, battleId);
            
            await _cache.RemoveAsync($"battle:{playerId}");
        }
        

        await base.OnDisconnectedAsync(exception);
    }
    
    private string? GetCurrentPlayerId()
    {
        return Context.User?.FindFirstValue(PlayerIdClaim);
    }

    private string GetBattleCacheKey(string playerId)
    {
        return $"{BattleCachePrefix}{playerId}";
    }

    public async Task SendBattleError(string message)
    {
        await Clients.Caller.SendAsync("ReceiveBattleErrorMessage",message);
    }
}