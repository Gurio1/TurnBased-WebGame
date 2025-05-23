using Game.Core.PlayerProfile;
using Game.Core.PlayerProfile.ValueObjects;

namespace Game.Features.Monsters.Create;

public sealed record Request(string Name, Stats Stats, Dictionary<string, float> DropsTable, List<string> AbilityIds);
