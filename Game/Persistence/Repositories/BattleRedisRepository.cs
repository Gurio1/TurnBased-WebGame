using Game.Application.SharedKernel;
using Game.Core.Battle;
using Game.Core.Battle.PVE;
using Game.Persistence.Redis;
using Newtonsoft.Json;

namespace Game.Persistence.Repositories;

public sealed class BattleRedisRepository : IBattleRepository
{
    private readonly RedisProvider redisProvider;
    
    public BattleRedisRepository(RedisProvider redisProvider) => this.redisProvider = redisProvider;
    
    public Task<ResultWithoutValue> Delete(string battleId)
    {
        var db = redisProvider.GetDatabase();
        
        bool result = db.KeyDelete(battleId);
        
        return Task.FromResult(
            result
                ? ResultWithoutValue.Success()
                : ResultWithoutValue.Failure($"Can't delete battle with id '{battleId}' from Redis"));
    }
    
    public Task<ResultWithoutValue> Save(PveBattle battle)
    {
        var db = redisProvider.GetDatabase();
        
        string jsonBattle = JsonConvert.SerializeObject(battle, Formatting.Indented,
            new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        
        bool createResult = db.StringSet(battle.Id, jsonBattle);
        
        return Task.FromResult(
            createResult
                ? ResultWithoutValue.Success()
                : ResultWithoutValue.Failure("Unable to save battle to Redis"));
    }
}
