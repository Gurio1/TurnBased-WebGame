using Game.Core;
using Game.Core.Models;
using Game.Features.Battle.Models;
using Game.Features.Monsters;
using Game.Features.Players;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace Game.Features.Battle;

public class BattleRedisRepository : IBattleRepository
{
    
    //TODO : Handle exceptions
    private readonly RedisProvider _redisProvider;
    private readonly IPlayersMongoRepository _playersMongoRepository;
    private readonly IMonstersMongoRepository _monstersMongoRepository;

    public BattleRedisRepository(RedisProvider redisProvider,IPlayersMongoRepository playersMongoRepository,IMonstersMongoRepository monstersMongoRepository)
    {
        _redisProvider = redisProvider;
        _playersMongoRepository = playersMongoRepository;
        _monstersMongoRepository = monstersMongoRepository;
    }

    public async Task<Result<PveBattle>> CreateBattleAsync(Player player)
    {
        var monsterResult = await _monstersMongoRepository.GetByNameWithAbilities("Goblin");

        if (monsterResult.IsFailure)
        {
            return monsterResult.AsError<PveBattle>();
        }

        var battle = new PveBattle(player, monsterResult.Value,new());

        player.BattleId = battle.Id;
        
        battle.UpdatePlayer(player);
        
        await SaveBattleData(battle);
        
        var update = Builders<Player>.Update.Set(p => p.BattleId, battle.Id);

        await _playersMongoRepository.UpdateOneAsync(player.Id,update);

        return Result<PveBattle>.Success(battle);
    }

    public Task<Result<PveBattle>> GetActiveBattleAsync(string battleId)
    {
        var db =_redisProvider.GetDatabase();

        var getResult = db.StringGet(battleId);

        if (getResult.IsNull)
        {
            //TODO: Here we should go in mongo if battle was moved from cache
            //TODO: Eventually implement cache time limit 
            return Task.FromResult(Result<PveBattle?>.Failure("NotImplemented"));
        }

        var battle = JsonConvert.DeserializeObject<PveBattle>(getResult.ToString(), new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });
        
        return Task.FromResult(
            battle is null
            ? Result<PveBattle>.Failure($"Could not deserialize battle with id '{battleId}'")
            : Result<PveBattle>.Success(battle)
            );
    }
    
    public Task<ResultWithoutValue> SaveBattleData(PveBattle pveBattle)
    {
        var db =_redisProvider.GetDatabase();
        
        var jsonBattle = JsonConvert.SerializeObject(pveBattle, Formatting.Indented, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });

        var createResult = db.StringSet(pveBattle.Id, jsonBattle);

        return Task.FromResult(
            createResult
                ? ResultWithoutValue.Success()
                : ResultWithoutValue.Failure(new Error("500","Unable to save battle to Redis")));
    }

    public async Task<bool> RemoveBattle(string battleId)
    {
        var db =_redisProvider.GetDatabase();


        var result =  await db.KeyDeleteAsync(battleId);

        return result;
    }
}