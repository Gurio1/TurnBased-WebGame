using Game.Core.Abilities;
using Game.Core.Models;

namespace Game.Features.Abilities.Contracts;

public class AbilityHomeViewModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Cooldown { get; set; }
    public string Description { get; set; }
}

public static class Mapper
{
    public static AbilityHomeViewModel ToAbilityHomeViewModel(this Ability ability, Player abilityOwner) =>
        new()
        {
            Id = ability.Id,
            Name = ability.Name,
            Cooldown = ability.Cooldown,
            Description = ability.GetAbilityDescription(abilityOwner)
        };
}
