using Game.Core.Abilities;
using Game.Core.Models;
using Game.Core.Monsters;
using Game.Core.Rewards;
using Game.Drop;
using MediatR;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Game.Battle;

internal class BattleManager
{
    private readonly RedisConnectionFactory _redisConnectionFactory;
    private readonly IMediator _mediator;
    private readonly DropService _dropService;

    public BattleManager(RedisConnectionFactory redisConnectionFactory,IMediator mediator,DropService dropService)
    {
        _redisConnectionFactory = redisConnectionFactory;
        _mediator = mediator;
        _dropService = dropService;
    }

    public async Task<Battle> GetOrCreate(Hero character)
    {
        var getResult = await GetBattleData(character.Id);

        if (!getResult.IsNull)
        {
            var res = JsonConvert.DeserializeObject<Battle>(getResult.ToString(), new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            })!;
            return res;
        }
        
        
        var battle = new Battle(){Hero = character,Enemy = new Goblin(){Abilities = [new BaseAttack()]}};

        return await SaveBattleData(battle);

    }

    public async Task<IReward?> UseHeroAbility(int abilityId, Battle battle)
    {
        battle.Hero.DoAction(abilityId, battle.Enemy,_mediator);

        if (battle.Enemy.Hp <= 0)
        {
            var drop = _dropService.GiveDrop(battle.Enemy);
            
            //drop.HandleDrop(battle.Hero);
            
            return new BattleReward() { Gold = 20, Experience = 5, Drop = drop };
        }
        
        battle.Enemy.DoAction(0,battle.Hero,_mediator);
        
        await SaveBattleData(battle);

        return null;
    }


    public async Task<RedisValue> GetBattleData(Guid id)
    {
        var connection = _redisConnectionFactory.CreateConnection();
        var db = connection.GetDatabase();
        
        var getResult = db.StringGet(id.ToString());
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

        var createResult = db.StringSet(battle.Hero.Id.ToString(), jsonBattle);
        
        await db.Multiplexer.CloseAsync();

        if (createResult)
        {
            return battle;
        }
        
        throw new Exception("DAAmn,i cant save battle in Redis");
    }
}