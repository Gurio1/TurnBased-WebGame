using Game.Core.Models;
using Game.Features.Battle.Contracts;

namespace Game.Features.Battle;

public class Battle
{
    public HeroBattleModel Hero { get; set; }
    public Monster Enemy { get; set; }
}