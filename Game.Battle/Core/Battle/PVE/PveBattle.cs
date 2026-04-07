using Game.Battle.Core.Battle.PVE.Events;
using Game.Battle.Core.Models;
using Game.SharedKernel.Models;
using Game.SharedKernel.Results;
using Newtonsoft.Json;

namespace Game.Battle.Core.Battle.PVE;

public class PveBattle : Entity
{
    public string Id { get; }
    public Player Player { get; }
    public Monster Monster { get; }
    
    private PveBattle(Player player, Monster monster)
    {
        Id = Guid.NewGuid().ToString();
        Player = player;
        Player.BattleId = Id;
        Monster = monster;
    }
    
    // Json.NET will use this
    [JsonConstructor]
    private PveBattle(string id, Player player, Monster monster)
    {
        Id = id;
        Player = player;
        Monster = monster;
    }
    
    public static Result<PveBattle> Create(Result<Player> playerResult, Result<Monster> monsterResult)
    {
        if (playerResult.IsFailure)
            return playerResult.AsError<PveBattle>();
        
        if (playerResult.Value.BattleId is not null)
            return Result<PveBattle>.Invalid("Can't create new battle.Player is already in the battle");
        
        if (monsterResult.IsFailure)
            return monsterResult.AsError<PveBattle>();

        playerResult.Value.LoadAbilitiesFromIds();
        monsterResult.Value.LoadAbilitiesFromIds();
        
        return Result<PveBattle>.Success(new PveBattle(playerResult.Value, monsterResult.Value));
    }
    
    public ResultWithoutValue ExecuteTurn(string abilityId, BattleContext battleContext)
    {
        battleContext.SetBattleId(Id);
        
        Player.UseAbility(abilityId, Monster, battleContext);
        
        if (Monster.IsDead())
        {
            AddDomainEvent(new PveBattleWon(Player, Monster));
            return ResultWithoutValue.Success();
        }
        
        Monster.UseAbility("0", Player, battleContext);
        
        if (Player.IsDead()) AddDomainEvent(new PveBattleLost(Player));
        
        return ResultWithoutValue.Success();
    }
}
