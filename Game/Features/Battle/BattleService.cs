using Game.Core.Models;
using Game.Core.Monsters;
using Game.Features.Abilities;
using Game.Features.Players;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Game.Features.Battle;

public class BattleService
{
    
    //TODO : Handle exceptions
    private readonly RedisConnectionFactory _redisConnectionFactory;
    private readonly PlayersService _playersService;

    public BattleService(RedisConnectionFactory redisConnectionFactory,PlayersService playersService)
    {
        _redisConnectionFactory = redisConnectionFactory;
        _playersService = playersService;
    }

    private async Task<Battle> CreateAsync(Hero player)
    {
        
        
        var battle = new Battle(){Hero = player,Enemy = new Goblin(){AbilityIds = ["0"]}};

        player.BattleId = battle.Id;
        
        await SaveBattleData(battle);

        await _playersService.UpdateAsync(player);

        return battle;
    }

    public async Task<Battle?> GetOrCreate(string playerId)
    {
        var player = await _playersService.GetByIdWithAbilities(playerId);
        
        if (!player.InBattle()) return await CreateAsync(player);


        var getResult = await GetBattleData(player.BattleId!);
       

        if (!getResult.IsNull)
        {
            return JsonConvert.DeserializeObject<Battle>(getResult.ToString(), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            })!;
        }
        
        
        //TODO: Here we should go in mongo if battle was moved from cache
        //TODO: Eventually implement cache time limit 

        return null;
    }
    
    public async Task<RedisValue> GetBattleData(string id)
    {
        var connection = _redisConnectionFactory.CreateConnection();
        var db = connection.GetDatabase();

        var getResult = db.StringGet(id);
        await db.Multiplexer.CloseAsync();

        return getResult;
    }
    public async Task<Battle> SaveBattleData(Battle battle)
    {
        var connection = _redisConnectionFactory.CreateConnection();
        var db = connection.GetDatabase();
        
        var jsonBattle = JsonConvert.SerializeObject(battle, Formatting.Indented, new JsonSerializerSettings
        {
            TypeNameHandling = TypeNameHandling.Auto
        });

        var createResult = db.StringSet(battle.Id, jsonBattle);
        
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