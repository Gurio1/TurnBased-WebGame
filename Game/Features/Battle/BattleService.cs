using Game.Core.Monsters;
using Game.Features.Abilities;
using Game.Features.Battle.Contracts;
using Game.Features.Players;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Game.Features.Battle;

public class BattleService
{
    private readonly RedisConnectionFactory _redisConnectionFactory;
    private readonly PlayersService _playersService;
    private readonly IAbilityService _abilityService;

    public BattleService(RedisConnectionFactory redisConnectionFactory,PlayersService playersService,IAbilityService abilityService)
    {
        _redisConnectionFactory = redisConnectionFactory;
        _playersService = playersService;
        _abilityService = abilityService;
    }
    
    public async Task<Battle> GetOrCreate(string playerId)
    {
        var getResult = await GetBattleData(playerId);

        if (!getResult.IsNull)
        {
            var res = JsonConvert.DeserializeObject<Battle>(getResult.ToString(), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            })!;
            return res;
        }

        var player =  await _playersService.CreateQuery()
            .GetById(playerId)
            .WithAbilities()
            .ExecuteAsync<HeroBattleModel>();
        
        var battle = new Battle(){Hero = player,Enemy = new Goblin(){AbilityIds = ["0"]}};

        return await SaveBattleData(battle);

    }
    
    public async Task<RedisValue> GetBattleData(string id)
    {
        var connection = _redisConnectionFactory.CreateConnection();
        var db = connection.GetDatabase();

        var getResult = db.StringGet(id);
        await db.Multiplexer.CloseAsync();

        return getResult;
    }
    public async Task<Features.Battle.Battle> SaveBattleData(Features.Battle.Battle battle)
    {
        var connection = _redisConnectionFactory.CreateConnection();
        var db = connection.GetDatabase();
        
        var jsonBattle = JsonConvert.SerializeObject(battle, Formatting.Indented, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });

        var createResult = db.StringSet(battle.Hero.Id, jsonBattle);
        
        await db.Multiplexer.CloseAsync();

        if (createResult)
        {
            return battle;
        }
        
        throw new Exception("DAAmn,i cant save battle in Redis");
    }

    public async Task<bool> RemoveBattle(string battleId)
    {
        var connection = _redisConnectionFactory.CreateConnection();
        var db = connection.GetDatabase();


        var result =  await db.KeyDeleteAsync(battleId);
        await db.Multiplexer.CloseAsync();

        return result;
    }
}