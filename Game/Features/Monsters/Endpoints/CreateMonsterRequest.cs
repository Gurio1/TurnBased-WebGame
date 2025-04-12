using Game.Core.Models;

namespace Game.Features.Monsters.Endpoints;

public record CreateMonsterRequest(string Name,Stats Stats,Dictionary<string,float> DropsTable,string[] AbilityIds);