using Game.Core.Models;
using Game.Core.Rewards;
using Game.Features.Abilities;
using Game.Features.Battle.Contracts;
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
    private readonly IAbilityService _abilityService;

    public BattleManager(IMediator mediator,DropService dropService,PlayersService playersService,BattleService battleService,IAbilityService abilityService)
    {
        _mediator = mediator;
        _dropService = dropService;
        _playersService = playersService;
        _battleService = battleService;
        _abilityService = abilityService;
    }


    public async Task<IReward?> UseHeroAbility(string abilityId, Features.Battle.Battle battle)
    {
        var ability = battle.Hero.Abilities.FirstOrDefault(a => a.Id == abilityId);

        if (ability is null)
        {
            return null;
        }
        
        DecreaseAbilityCooldowns(battle);

        //TODO: I dont save hero data.Should be fixed
        var hero = battle.Hero.ToModel();
        ExecuteDebuffs(hero,_mediator);
        
        battle.Enemy.Defence(hero,ability,_mediator);
        
        if (battle.Enemy.Hp <= 0)
        {
            var drop = await _dropService.HandleDrop(battle.Enemy);

            if (drop is not null)
            {
                hero.Inventory.Add(drop);
            }
            
            await _playersService.UpdateAsync(battle.Hero.Id,hero);

            await _battleService.RemoveBattle(battle.Hero.Id);
            
            return new BattleReward() { Gold = 20, Experience = 5, Drop = drop };
        }
        
        await _battleService.SaveBattleData(battle);

        return null;
    }
    
    private void DecreaseAbilityCooldowns(Features.Battle.Battle battle)
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