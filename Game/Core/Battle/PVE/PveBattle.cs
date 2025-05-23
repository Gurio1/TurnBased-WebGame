using Game.Application.SharedKernel;
using Game.Core.Battle.PVE.Events;
using Game.Core.Models;
using Microsoft.AspNetCore.Http.HttpResults;

namespace Game.Core.Battle.PVE;

public class PveBattle : Entity
{
    private PveBattle() { }
    
    public static Result<PveBattle> Create(Result<CombatPlayer> playerResult, Result<Monster> monsterResult)
    {
        if (playerResult.IsFailure)
            return playerResult.AsError<PveBattle>();
        
        if (playerResult.Value.BattleId is not null)
            return Result<PveBattle>.Invalid("Can't create new battle.Player is already in the battle");
        
        if (monsterResult.IsFailure)
            return monsterResult.AsError<PveBattle>();
        
        return Result<PveBattle>.Success(new PveBattle()
        {
            Monster = monsterResult.Value, CombatPlayer = playerResult.Value
        });
    }
    
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public CombatPlayer CombatPlayer { get; private init; }
    public Monster Monster { get; private init; }
    
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
