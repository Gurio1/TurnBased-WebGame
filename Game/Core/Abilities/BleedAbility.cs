using Game.Core.AbilityEffects;
using Game.Core.Models;
using MediatR;

namespace Game.Core.Abilities;

internal class BleedAbility : Ability
{
    public override string TypeName { get; init; } = nameof(BleedAbility);
    public override int Id { get; set; } = 1;
    public override string Name { get; set; } = "Bleed";
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

        target.SetDebuff(new Bleed(Duration),mediator);

        CurrentCooldown = Cooldown;

        return damage;

    }
}