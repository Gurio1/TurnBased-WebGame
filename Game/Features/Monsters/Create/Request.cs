using Game.Core.Models;

namespace Game.Features.Monsters.CreateMonster;

public sealed record Request(string Name, Stats Stats, Dictionary<string, float> DropsTable, List<string> AbilityIds);
