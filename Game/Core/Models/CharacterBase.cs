using Game.Core.Abilities;
using Game.Core.AbilityEffects;
using Game.Core.Equipment;
using Game.Logger;
using MediatR;

namespace Game.Core.Models;

public abstract class CharacterBase
{
    public abstract float Hp { get; set; }
    public float Armor { get; set; } = 5;
    public float Damage { get; set; } = 25;
    public float DebuffResistance { get; set; }

    public float CritChance { get; } = 0.1f;
    public float CritDamage { get; } = 1.5f;
    public float DodgeChance { get; } = 0.1f;

    private bool _canDoAction = true;

    public abstract List<Ability> Abilities { get; set; } 
    public abstract List<IEquipment> Equipment { get; set; }

    public abstract List<IDebuff> Debuffs { get; set; } 

    private static readonly Random Rnd = new();

    public abstract string CharacterType { get; }

    public void EquipEquipment(IEquipment equipment)
    {
        var existedEq = Equipment.FirstOrDefault(e => e.EquipmentType == equipment.EquipmentType);
        
        if (existedEq != null)
        {
            Equipment.Remove(existedEq);
            existedEq.RemoveStats(this);
        }
        
        equipment.ApplyStats(this);
    }

    public void UnequipEquipment(IEquipment equipment)
    {
        if (Equipment.Remove(equipment))
        {
            equipment.RemoveStats(this);
        }
    }

    public float CalculateDamage(float damage,IMediator mediator,float damageMultiplier = 1f)
    {
        var finalDamage = damage * damageMultiplier;
        var rnd = (float)Rnd.NextDouble();

        if (!(rnd <= CritChance)) return finalDamage;
        
        finalDamage *= CritDamage;
        mediator.Publish(new ActionLogNotification($"{CharacterType} did critical hit!"));
        return finalDamage;
    }

    protected void Defence(CharacterBase attacker, Ability ability, IMediator mediator)
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
    
    private void UseAbility(int id, CharacterBase target, IMediator mediator)
    {
        var abilityToExecute = Abilities.First(a => a.Id == id);

        target.Defence(this,abilityToExecute,mediator);
    }
    
    public void DoAction(int id, CharacterBase target, IMediator mediator)
    {
        DecreaseAbilityCooldowns();
        ExecuteDebuffs(mediator);
        
        if (_canDoAction)
        {
            UseAbility(id,target,mediator);
        }
        else
        {
            mediator.Publish(new ActionLogNotification($"{CharacterType} skipped turn"));
        }

        _canDoAction = true;
    }

    private void DecreaseAbilityCooldowns()
    {
        Abilities.ForEach(x => x.DecreaseCurrentCooldown());
    }

    public void SkipAction()
    {
        _canDoAction = false;
    }

    public void SetDebuff(IDebuff debuff,IMediator mediator)
    {
        mediator.Publish(new ActionLogNotification($"{CharacterType} has been debuffed with {debuff.Name}"));
        Debuffs.Add(debuff);
    }

    private void ExecuteDebuffs(IMediator mediator)
    {
        for (int i = Debuffs.Count - 1; i >= 0; i--)
        {
            var debuff = Debuffs[i];
            debuff.Execute(this,mediator);

            if (debuff.Duration <= 0)
            {
                Debuffs.RemoveAt(i);
            }
        }

    }
    
    
}