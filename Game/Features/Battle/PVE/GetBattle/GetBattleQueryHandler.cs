using Game.Application.SharedKernel;
using Game.Core.Battle.PVE;
using Game.Persistence.Mongo;
using Game.Persistence.Redis;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Game.Features.Battle.PVE.GetBattle;

public sealed class GetBattleQueryHandler : IRequestHandler<GetBattleQuery, Result<PveBattle>>
{
    private readonly IMongoCollectionProvider mongoCollectionProvider;
    private readonly RedisProvider redisProvider;
    
    public GetBattleQueryHandler(RedisProvider redisProvider, IMongoCollectionProvider mongoCollectionProvider)
    {
        this.redisProvider = redisProvider;
        this.mongoCollectionProvider = mongoCollectionProvider;
    }
    
    public async Task<Result<PveBattle>> Handle(GetBattleQuery request, CancellationToken cancellationToken)
    {
        var db = redisProvider.GetDatabase();
        
        var getResult = db.StringGet(request.BattleId);
        
        if (getResult.IsNull)
        {
            var battleFromMongo = await mongoCollectionProvider
                .GetCollection<PveBattle>()
                .Find(b => b.Id == request.BattleId)
                .FirstOrDefaultAsync(cancellationToken);
            
            return battleFromMongo is null
                ? Result<PveBattle>.NotFound($"Battle with id '{request.BattleId}' doesn't exist")
                : Result<PveBattle>.Success(battleFromMongo);
        }
        
        var battle = JsonConvert.DeserializeObject<PveBattle>(getResult.ToString(),
            new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });
        
        return battle is null
            ? Result<PveBattle>.Failure($"Could not deserialize battle with id '{request.BattleId}'")
            : Result<PveBattle>.Success(battle);
    }
}
