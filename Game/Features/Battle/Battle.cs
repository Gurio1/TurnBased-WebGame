using Game.Core.Models;

namespace Game.Features.Battle;

public class Battle
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public Hero Hero { get; set; }
    public Monster Enemy { get; set; }
}