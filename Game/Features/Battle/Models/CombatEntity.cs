using Game.Core.Equipment;
using Game.Core.Models;
using Game.Core.StatusEffects;

namespace Game.Features.Battle.Models;

public abstract class CombatEntity
{
    private static readonly Random Rnd = new();
    public string Id { get; set; }
    public string Name { get; set; }
    public Stats Stats { get; set; }
    public Dictionary<string, EquipmentBase?> Equipment { get; set; } = [];
    
    public List<IDebuff> Debuffs { get; set; } = [];
    
    public virtual string CharacterType { get; set; }
    
    public bool IsDead() => Stats.CurrentHealth <= 0;
    
    public virtual float CalculateDamage(float damage, BattleContext context, float damageMultiplier = 1f)
    {
        float finalDamage = damage * damageMultiplier;
        float rnd = (float)Rnd.NextDouble();
        
        if (!(rnd <= Stats.CriticalChance)) return finalDamage;
        
        finalDamage *= Stats.CriticalDamage;
        
        Console.WriteLine($"{CharacterType} did critical hit!");
        context.PublishActionLog($"{CharacterType} did critical hit!");
        return finalDamage;
    }
    
    public virtual bool Defence(float damage, BattleContext context)
    {
        if (Rnd.NextDouble() <= Stats.DodgeChance)
        {
            context.PublishActionLog($"{CharacterType} dodged");
            return false;
        }
        
        Stats.CurrentHealth -= damage;
        context.PublishActionLog($"{CharacterType} took {damage} damage.");
        
        return true;
    }
}
