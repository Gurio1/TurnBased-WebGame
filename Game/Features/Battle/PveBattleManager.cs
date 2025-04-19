using Game.Core.Common;
using Game.Features.Battle.Models;
using Game.Features.Battle.PVE.Events;

namespace Game.Features.Battle;

public class PveBattleManager
{
    private readonly BattleContext battleContext;
    private readonly IBattleRepository battleRedisRepository;
    private readonly IDispatcher dispatcher;


    public PveBattleManager(BattleContext battleContext,IBattleRepository battleRedisRepository,IDispatcher dispatcher)
    {
        this.battleContext = battleContext;
        this.battleRedisRepository = battleRedisRepository;
        this.dispatcher = dispatcher;
    }


    public async Task<ResultWithoutValue> ExecutePlayerTurnAsync(string abilityId, PveBattle pveBattle)
    {
        battleContext.SetBattleId(pveBattle.Id);
        
        
        var ability = pveBattle.CombatPlayer.Abilities.FirstOrDefault(a => a.Id == abilityId);

        if (ability is null)
        {
           return ResultWithoutValue.Failure(new CustomError("400",$"The player '{pveBattle.CombatPlayer.Id}' doesnt have ability with id '{abilityId}'"));
        }

        var player = pveBattle.CombatPlayer;
        
        DecreaseAbilityCooldowns(pveBattle);

        ExecuteDebuffs(pveBattle.CombatPlayer,battleContext);
        
        ability.Execute(player,pveBattle.Monster,battleContext);
        
        
        if (pveBattle.Monster.IsDead())
        {
            return await dispatcher.Dispatch(new DefeatMonsterCommand(pveBattle.Monster, pveBattle.CombatPlayer));
        }

        ExecuteDebuffs(pveBattle.Monster,battleContext);

        var enemyAbility = pveBattle.Monster.Abilities.FirstOrDefault();
        
        enemyAbility!.Execute(pveBattle.Monster,player,battleContext);

        if (player.IsDead())
        {
           return await dispatcher.Dispatch(new DefeatPlayerCommand(pveBattle.CombatPlayer));
        }
        
        //TODO : Log if any error
        await dispatcher.Dispatch(new SendBattleDataCommand(pveBattle));
        
        //TODO: Create command. Get rid of Repositories
        await battleRedisRepository.SaveBattleData(pveBattle);
        
        return ResultWithoutValue.Success();
    }
    
    private static void  DecreaseAbilityCooldowns(PveBattle pveBattle) => 
        pveBattle.CombatPlayer.Abilities.ForEach(x => x.DecreaseCurrentCooldown());
    
    private static void ExecuteDebuffs(CombatEntity target,BattleContext battleContext)
    {
        for (int i = target.Debuffs.Count - 1; i >= 0; i--)
        {
            var debuff = target.Debuffs[i];
            debuff.Execute(target,battleContext);

            if (debuff.Duration <= 0)
            {
                target.Debuffs.RemoveAt(i);
            }
        }

    }
}
