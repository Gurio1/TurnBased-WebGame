using Game.Core.Models;
using Game.Core.Rewards;
using Game.Features.Drop;
using Game.Features.Players;
using MediatR;

namespace Game.Features.Battle;

public class BattleManager
{
    private readonly IMediator _mediator;
    private readonly DropService _dropService;
    private readonly PlayersService _playersService;
    private readonly BattleService _battleService;

    public BattleManager(IMediator mediator,DropService dropService,PlayersService playersService,BattleService battleService)
    {
        _mediator = mediator;
        _dropService = dropService;
        _playersService = playersService;
        _battleService = battleService;
    }


    public async Task<IReward?> UseHeroAbility(string abilityId, Battle battle)
    {
        var ability = battle.Hero.Abilities.FirstOrDefault(a => a.Id == abilityId);

        if (ability is null)
        {
            return null;
        }
        
        DecreaseAbilityCooldowns(battle);

        ExecuteDebuffs(battle.Hero,_mediator);
        
        battle.Enemy.Defence(battle.Hero,ability,_mediator);
        
        if (battle.Enemy.Hp <= 0)
        {
            var drop = await _dropService.HandleDrop(battle.Enemy);

            if (drop is not null)
            {
                battle.Hero.Inventory.Add(drop);
            }
            
            await _battleService.RemoveBattle(battle.Id);

            battle.Hero.BattleId = null;
            
            await _playersService.UpdateAsync(battle.Hero);
            
            return new BattleReward() { Gold = 20, Experience = 5, Drop = drop };
        }
        
        await _battleService.SaveBattleData(battle);

        return null;
    }
    
    private void DecreaseAbilityCooldowns(Battle battle)
    {
        battle.Hero.Abilities.ForEach(x => x.DecreaseCurrentCooldown());
    }
    private void ExecuteDebuffs(CharacterBase target,IMediator mediator)
    {
        for (int i = target.Debuffs.Count - 1; i >= 0; i--)
        {
            var debuff = target.Debuffs[i];
            debuff.Execute(target,mediator);

            if (debuff.Duration <= 0)
            {
                target.Debuffs.RemoveAt(i);
            }
        }

    }
    
    
}