using Game.Core.Models;
using Game.Features.Battle.Models;
using Game.Features.Battle.PVE.Events;
using MediatR;

namespace Game.Features.Battle;

public class PveBattleManager
{
    private readonly BattleContext battleContext;
    private readonly IBattleRepository battleRedisRepository;
    private readonly IMediator mediator;


    public PveBattleManager(BattleContext battleContext,IBattleRepository battleRedisRepository,IMediator mediator)
    {
        this.battleContext = battleContext;
        this.battleRedisRepository = battleRedisRepository;
        this.mediator = mediator;
    }


    public async Task ExecutePlayerTurnAsync(string abilityId, PveBattle pveBattle)
    {
        battleContext.SetBattleId(pveBattle.Id);
        
        
        var ability = pveBattle.CombatPlayer.Abilities.FirstOrDefault(a => a.Id == abilityId);

        if (ability is null)
        {
            return;
        }

        var player = pveBattle.CombatPlayer;
        
        DecreaseAbilityCooldowns(pveBattle);

        ExecuteDebuffs(pveBattle.CombatPlayer,battleContext);
        
        ability.Execute(player,pveBattle.Monster,battleContext);
        
        
        if (pveBattle.Monster.IsDead())
        {
            await mediator.Publish(new MonsterDefeatedEvent(pveBattle.Monster, pveBattle.CombatPlayer));
            return;
        }

        ExecuteDebuffs(pveBattle.Monster,battleContext);

        var enemyAbility = pveBattle.Monster.Abilities.FirstOrDefault();
        
        enemyAbility!.Execute(pveBattle.Monster,player,battleContext);

        if (player.IsDead())
        {
            await mediator.Publish(new PlayerDefeatedEvent(pveBattle.CombatPlayer));
            return;
        }
        
        await mediator.Publish(new PveBattleDataSentEvent(pveBattle));
        await battleRedisRepository.SaveBattleData(pveBattle);
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
