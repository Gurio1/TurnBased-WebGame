using Game.Core.Models;

namespace Game.Features.Monsters.CreateMonster;

public record CreateMonsterRequest(string Name,Stats Stats,Dictionary<string,float> DropsTable,string[] AbilityIds);