using Game.Core.SharedKernel;
using Game.Persistence.Redis;
using Newtonsoft.Json;

namespace Game.Features.Battle.PVE;

public sealed class SaveBattleInRedisCommandHandler : IRequestHandler<SaveBattleInRedisCommand, ResultWithoutValue>
{
    private readonly RedisProvider redisProvider;
    
    public SaveBattleInRedisCommandHandler(RedisProvider redisProvider) => this.redisProvider = redisProvider;
    
    public Task<ResultWithoutValue> Handle(SaveBattleInRedisCommand request, CancellationToken cancellationToken)
    {
        var db = redisProvider.GetDatabase();
        
        string jsonBattle = JsonConvert.SerializeObject(request.Battle, Formatting.Indented,
            new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });
        
        bool createResult = db.StringSet(request.Battle.Id, jsonBattle);
        
        return Task.FromResult(
            createResult
                ? ResultWithoutValue.Success()
                : ResultWithoutValue.Failure("Unable to save battle to Redis"));
    }
}
