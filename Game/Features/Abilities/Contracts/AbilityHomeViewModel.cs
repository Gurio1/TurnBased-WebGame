using Game.Core.Abilities;
using Game.Core.PlayerProfile;
using Game.Core.PlayerProfile.Aggregates;

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
    public static AbilityHomeViewModel ToAbilityHomeViewModel(this Ability ability, GamePlayer abilityOwner) =>
        new()
        {
            Id = ability.Id,
            Name = ability.Name,
            Cooldown = ability.Cooldown,
            Description = ability.GetAbilityDescription(abilityOwner)
        };
}
