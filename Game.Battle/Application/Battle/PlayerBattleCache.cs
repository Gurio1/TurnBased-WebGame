using Microsoft.Extensions.Caching.Distributed;

namespace Game.Battle.Application.Battle;

public sealed class PlayerBattleCache
{
    private const string BattleCachePrefix = "battle:";
    
    private readonly IDistributedCache cache;
    
    public PlayerBattleCache(IDistributedCache cache) => this.cache = cache;
    
    private static string GetBattleCacheKey(string playerId) => $"{BattleCachePrefix}{playerId}";
    
    public async Task SetBattleIdCache(string playerId,string battleId) =>
        await cache.SetStringAsync(GetBattleCacheKey(playerId), battleId,
            new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(30) });
    
    public async Task<string?> GetBattleId(string playerId) => await cache.GetStringAsync(GetBattleCacheKey(playerId));
    public async Task Remove(string playerId) => await cache.RemoveAsync(GetBattleCacheKey(playerId));
}
