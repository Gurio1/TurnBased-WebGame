using Game.Core.Models;
using Game.Features.Battle.Models;
using Game.Features.Battle.PVE.Events;
using MediatR;

namespace Game.Features.Battle;

public class PveBattleManager
{
    private readonly BattleContext _battleContext;
    private readonly IBattleRepository _battleRedisRepository;
    private readonly IMediator _mediator;


    public PveBattleManager(BattleContext battleContext,IBattleRepository battleRedisRepository,IMediator mediator)
    {
        _battleContext = battleContext;
        _battleRedisRepository = battleRedisRepository;
        _mediator = mediator;
    }


    public async Task ExecutePlayerTurnAsync(string abilityId, PveBattle pveBattle)
    {
        _battleContext.SetBattleId(pveBattle.Id);
        
        
        var ability = pveBattle.CombatPlayer.Abilities.FirstOrDefault(a => a.Id == abilityId);

        if (ability is null)
        {
            return;
        }

        CombatPlayer player = pveBattle.CombatPlayer;
        
        DecreaseAbilityCooldowns(pveBattle);
        
        ExecuteDebuffs(pveBattle.CombatPlayer,_battleContext);
        
        ability.Execute(player,pveBattle.Monster,_battleContext);
        
        
        if (pveBattle.Monster.IsDead())
        {
            await _mediator.Publish(new MonsterDefeatedEvent(pveBattle.Monster, pveBattle.CombatPlayer));
            return;
        }
        
        ExecuteDebuffs(pveBattle.Monster,_battleContext);

        var enemyAbility = pveBattle.Monster.Abilities.FirstOrDefault();
        
        enemyAbility!.Execute(pveBattle.Monster,player,_battleContext);

        if (player.IsDead())
        {
            await _mediator.Publish(new PlayerDefeatedEvent(pveBattle.CombatPlayer));
            return;
        }
        
        await _mediator.Publish(new PveBattleDataSentEvent(pveBattle));
        await _battleRedisRepository.SaveBattleData(pveBattle);
    }
    
    private static void  DecreaseAbilityCooldowns(PveBattle pveBattle)
    {
        pveBattle.CombatPlayer.Abilities.ForEach(x => x.DecreaseCurrentCooldown());
    }
    
    private void ExecuteDebuffs(CombatEntity target,BattleContext battleContext)
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