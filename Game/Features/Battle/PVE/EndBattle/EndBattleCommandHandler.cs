using Game.Core.SharedKernel;
using Game.Persistence.Redis;

namespace Game.Features.Battle.PVE.EndBattle;

public sealed class EndBattleCommandHandler : IRequestHandler<EndBattleCommand, ResultWithoutValue>
{
    private readonly RedisProvider redisProvider;
    
    public EndBattleCommandHandler(RedisProvider redisProvider) => this.redisProvider = redisProvider;
    
    public Task<ResultWithoutValue> Handle(EndBattleCommand request, CancellationToken cancellationToken)
    {
        var db = redisProvider.GetDatabase();
        
        bool result = db.KeyDelete(request.BattleId);
        
        return Task.FromResult(
            result
                ? ResultWithoutValue.Success()
                : ResultWithoutValue.Failure($"Can't delete battle with id '{request.BattleId}' from Redis"));
    }
}
