using Game.Application.SharedKernel;
using Game.Core.Battle.PVE.Events;
using Game.Core.Models;
using Newtonsoft.Json;

namespace Game.Core.Battle.PVE;

public class PveBattle : Entity
{
    public string Id { get; }
    public CombatPlayer CombatPlayer { get; }
    public Monster Monster { get; }
    
    private PveBattle(CombatPlayer player, Monster monster)
    {
        Id = Guid.NewGuid().ToString();
        CombatPlayer = player;
        CombatPlayer.BattleId = Id;
        Monster = monster;
    }
    
    // Json.NET will use this
    [JsonConstructor]
    private PveBattle(string id, CombatPlayer combatPlayer, Monster monster)
    {
        Id = id;
        CombatPlayer = combatPlayer;
        Monster = monster;
    }
    
    public static Result<PveBattle> Create(Result<CombatPlayer> playerResult, Result<Monster> monsterResult)
    {
        if (playerResult.IsFailure)
            return playerResult.AsError<PveBattle>();
        
        if (playerResult.Value.BattleId is not null)
            return Result<PveBattle>.Invalid("Can't create new battle.Player is already in the battle");
        
        if (monsterResult.IsFailure)
            return monsterResult.AsError<PveBattle>();
        
        return Result<PveBattle>.Success(new PveBattle(playerResult.Value, monsterResult.Value));
    }
    
    public ResultWithoutValue ExecuteTurn(string abilityId, BattleContext battleContext)
    {
        battleContext.SetBattleId(Id);
        
        CombatPlayer.UseAbility(abilityId, Monster, battleContext);
        
        if (Monster.IsDead())
        {
            AddDomainEvent(new PveBattleWon(CombatPlayer, Monster));
            return ResultWithoutValue.Success();
        }
        
        Monster.UseAbility("0", CombatPlayer, battleContext);
        
        if (CombatPlayer.IsDead()) AddDomainEvent(new PveBattleLost(CombatPlayer));
        
        return ResultWithoutValue.Success();
    }
}
