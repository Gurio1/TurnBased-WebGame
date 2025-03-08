using Game.Core.Equipment.Swords;
using Game.Core.Models;
using MediatR;

namespace Game.Core.Abilities;

public class BaseAttack : Ability
{
    public override int Id { get; set; } = 0;
    public override string TypeName { get; init; } = nameof(BaseAttack);
    public override string Name { get; set; } = "BaseAttack";
    public override int Cooldown { get; init; } = 0;
    public override int CurrentCooldown { get; set; } = 0;

    public override float Execute(CharacterBase owner, CharacterBase target, IMediator mediator)
    {
        var damage = owner.CalculateDamage(owner.Damage,mediator);
        return damage;
    }
    
}