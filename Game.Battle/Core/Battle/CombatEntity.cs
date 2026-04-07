using System.Text.Json.Serialization;
using Game.Battle.Core.Abilities;
using Game.Battle.Core.Models;
using Game.Core.Abilities;
using Game.Core.StatusEffects;
using Game.SharedKernel.Models;
using Game.SharedKernel.Results;
using Game.SharedKernel.Utilities;
using Game.SharedKernel.Utilities.Extensions;

namespace Game.Battle.Core.Battle;

public abstract class CombatEntity : Entity
{
    public string Id { get; set; }
    public string Name { get; set; }
    public Stats Stats { get; set; }
    public Ability[] Abilities { get; set; } = [];
    
    public List<string> AbilityIds { get; set; } = [];
    public Dictionary<string, Equipment?> Equipment { get; set; } = [];
    
    public List<IDebuff> Debuffs { get; set; } = [];
    
    public virtual string CharacterType { get; set; }
    
    public bool IsDead() => Stats.CurrentHealth <= 0;
    
    public ResultWithoutValue UseAbility(string abilityId, CombatEntity target, BattleContext battleContext)
    {
        var ability = Abilities.FirstOrDefault(a => a.Id == abilityId);
        
        if (ability is null)
            return ResultWithoutValue.Invalid($"The player '{Id}' doesnt have ability with id '{abilityId}'");
        
        DecreaseAbilityCooldowns();
        ExecuteDebuffs(battleContext);
        
        ability.Execute(this, target, battleContext);
        
        return ResultWithoutValue.Success();
    }
    
    public virtual float CalculateDamage(float damage, BattleContext context, float damageMultiplier = 1f)
    {
        float finalDamage = damage * damageMultiplier;
        int roll = RandomHelper.Instance.Next(0, 100);

        if (roll >= Stats.CriticalChance) return finalDamage;

        finalDamage = CriticalStatPercentages.ApplyCriticalDamageBonus(finalDamage, Stats.CriticalDamage);
        
        context.PublishActionLog($"{CharacterType} did critical hit!");
        return finalDamage;
    }
    
    public virtual bool Defence(float damage, BattleContext context)
    {
        if (RandomHelper.Instance.NextDouble() <= Stats.DodgeChance)
        {
            context.PublishActionLog($"{CharacterType} dodged");
            return false;
        }
        
        Stats.CurrentHealth -= damage;
        context.PublishActionLog($"{CharacterType} took {damage} damage.");
        
        return true;
    }
    
    public void DecreaseAbilityCooldowns() =>
        Array.ForEach(Abilities, x => x.DecreaseCurrentCooldown());

    public void LoadAbilitiesFromIds()
    {
        if (AbilityIds.Count == 0)
        {
            Abilities = [];
            return;
        }

        var abilitiesById = AbilityTypes.Discover()
            .Select(AbilityActivator.CreateAbility)
            .ToDictionary(ability => ability.Id, ability => ability);

        Abilities = AbilityIds
            .Select(id => abilitiesById.TryGetValue(id, out var ability) ? ability : null)
            .OfType<Ability>()
            .ToArray();
    }
    
    private void ExecuteDebuffs(BattleContext battleContext)
    {
        for (int i = Debuffs.Count - 1; i >= 0; i--)
        {
            var debuff = Debuffs[i];
            debuff.Execute(this, battleContext);
            
            if (debuff.Duration <= 0) Debuffs.RemoveAt(i);
        }
    }
}
