using Game.Core.Abilities;
using Game.Core.AbilityEffects;
using Game.Core.Equipment;
using Game.Logger;
using MediatR;

namespace Game.Core.Models;

public abstract class CharacterBase
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public abstract float Hp { get; set; }
    public abstract float Armor { get; set; }
    public abstract float Damage { get; set; } 
    public abstract float DebuffResistance { get; set; }

    public abstract float CriticalChance { get; set; } 
    public abstract float CriticalDamage { get; set; }
    public abstract float DodgeChance { get; set; } 


    public abstract List<string> AbilityIds { get; set; } 
    public abstract Dictionary<string, EquipmentBase?> Equipment { get; set; }

    public abstract List<IDebuff> Debuffs { get; set; } 

    private static readonly Random Rnd = new();

    public abstract string CharacterType { get; set; }

    public float CalculateDamage(float damage,IMediator mediator,float damageMultiplier = 1f)
    {
        var finalDamage = damage * damageMultiplier;
        var rnd = (float)Rnd.NextDouble();

        if (!(rnd <= CriticalChance)) return finalDamage;
        
        finalDamage *= CriticalDamage;
        mediator.Publish(new ActionLogNotification($"{CharacterType} did critical hit!"));
        return finalDamage;
    }

    public void Defence(CharacterBase attacker, Ability ability, IMediator mediator)
    {
        if (Rnd.NextDouble()<=DodgeChance)
        {
            mediator.Publish(new ActionLogNotification($"{CharacterType} dodged"));
            return;
        }

        var damage = ability.Execute(attacker, this,mediator);
        
        Hp -= damage;
        mediator.Publish(new ActionLogNotification($"{CharacterType} took {damage} damage."));
    }
    

    
    
    
}