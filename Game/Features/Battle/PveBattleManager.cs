using Game.Core.Models;
using Game.Core.SharedKernel;
using Game.Features.Battle.Models;
using Game.Features.Battle.PVE;
using Game.Features.Battle.PVE.Commands;
using Game.Features.Battle.PVE.EndBattle;

namespace Game.Features.Battle;

public class PveBattleManager
{
    private readonly BattleContext battleContext;
    private readonly IDispatcher dispatcher;
    
    
    public PveBattleManager(BattleContext battleContext, IDispatcher dispatcher)
    {
        this.battleContext = battleContext;
        this.dispatcher = dispatcher;
    }
    
    
    public async Task<ResultWithoutValue> ExecutePlayerTurnAsync(string abilityId, PveBattle pveBattle)
    {
        battleContext.SetBattleId(pveBattle.Id);
        
        
        var ability = pveBattle.CombatPlayer.Abilities.FirstOrDefault(a => a.Id == abilityId);
        
        if (ability is null)
            return ResultWithoutValue.CreateError(new CustomError("400",
                $"The player '{pveBattle.CombatPlayer.Id}' doesnt have ability with id '{abilityId}'"));
        
        var player = pveBattle.CombatPlayer;
        
        DecreaseAbilityCooldowns(pveBattle);
        
        ExecuteDebuffs(pveBattle.CombatPlayer, battleContext);
        
        ability.Execute(player, pveBattle.Monster, battleContext);
        
        
        if (pveBattle.Monster.IsDead())
        {
            await dispatcher.DispatchAsync(new DefeatMonsterCommand(pveBattle.Monster, pveBattle.CombatPlayer));
            return await dispatcher.DispatchAsync(new EndBattleCommand(pveBattle.Id));
        }
        
        ExecuteDebuffs(pveBattle.Monster, battleContext);
        
        var enemyAbility = pveBattle.Monster.Abilities.FirstOrDefault();
        
        enemyAbility!.Execute(pveBattle.Monster, player, battleContext);
        
        if (player.IsDead())
        {
            await dispatcher.DispatchAsync(new DefeatPlayerCommand(pveBattle.CombatPlayer));
            return await dispatcher.DispatchAsync(new EndBattleCommand(pveBattle.Id));
        }
        
        //TODO : Log if any error
        await dispatcher.DispatchAsync(new SendBattleDataCommand(pveBattle));
        
        var saveResult = await dispatcher.DispatchAsync(new SaveBattleInRedisCommand(pveBattle));
        
        return saveResult.IsFailure
            ? ResultWithoutValue.CreateError(saveResult.Error)
            : ResultWithoutValue.Success();
    }
    
    private static void DecreaseAbilityCooldowns(PveBattle pveBattle) =>
        pveBattle.CombatPlayer.Abilities.ForEach(x => x.DecreaseCurrentCooldown());
    
    private static void ExecuteDebuffs(CombatEntity target, BattleContext battleContext)
    {
        for (int i = target.Debuffs.Count - 1; i >= 0; i--)
        {
            var debuff = target.Debuffs[i];
            debuff.Execute(target, battleContext);
            
            if (debuff.Duration <= 0) target.Debuffs.RemoveAt(i);
        }
    }
}
