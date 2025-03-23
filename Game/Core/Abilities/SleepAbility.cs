using Game.Core.AbilityEffects;
using Game.Core.Models;
using Game.Logger;
using MediatR;

namespace Game.Core.Abilities;

public class SleepAbility : Ability
{
    public override string TypeName { get; init; } = nameof(SleepAbility);

    public override string Id { get; set; } = "2";
    public override string Name { get; set; } = "Sleep";
    public override int Cooldown { get; init; } = 4;
    public override int CurrentCooldown { get; set; }
    private int Duration { get; init; } = 2;
    public override float Execute(CharacterBase owner, CharacterBase target, IMediator mediator)
    {
        if (CurrentCooldown != 0)
        {
            Console.WriteLine("Can not use this ability");
            return 0;
        }

        var damage = owner.CalculateDamage(owner.Damage * 0.7f,mediator);

        SetDebuff(target,mediator);
        
        CurrentCooldown = Cooldown;
        
        return damage;
    }
    
    private void SetDebuff(CharacterBase target, IMediator mediator)
    {
        var debuff = new Sleep(Duration);
        target.Debuffs.Add(debuff);
        
        mediator.Publish(new ActionLogNotification($"{target.CharacterType} has been debuffed with {debuff.Name}"));
    }
}