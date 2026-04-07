using Game.Battle.Core.Battle;
using Game.Battle.Core.Battle.PVE;
using Game.SharedKernel.Results;
using Newtonsoft.Json;

namespace Game.Battle.Persistence.Cache.Redis;

public sealed class BattleRedisRepository(
    RedisProvider redisProvider,
    IBattleReadRepository battleReadRepository) : IBattleRepository
{
    public async Task<Result<PveBattle>> GetById(string battleId, CancellationToken ct = default)
    {
        var db = redisProvider.GetDatabase();
        var getResult = db.StringGet(battleId);

        if (getResult.IsNull)
            return await battleReadRepository.GetById(battleId, ct);

        var battle = JsonConvert.DeserializeObject<PveBattle>(
            getResult.ToString(),
            new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            });

        return battle is null
            ? Result<PveBattle>.Failure($"Could not deserialize battle with id '{battleId}'")
            : Result<PveBattle>.Success(battle);
    }

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

        string jsonBattle = JsonConvert.SerializeObject(
            battle,
            Formatting.Indented,
            new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.Auto });

        bool createResult = db.StringSet(battle.Id, jsonBattle);

        return Task.FromResult(
            createResult
                ? ResultWithoutValue.Success()
                : ResultWithoutValue.Failure("Unable to save battle to Redis"));
    }
}
