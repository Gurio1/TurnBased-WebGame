using Game.Core.Abilities;
using Game.Core.PlayerProfile.Aggregates;

namespace Game.Contracts;

public class AbilityHomeViewModel
{
    public string Id { get; set; }
    public string Name { get; set; }
    public int Cooldown { get; set; }
    public string Description { get; set; }
}
