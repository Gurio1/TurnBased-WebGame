using Game.Core.Battle.PVE;
using Game.Core.Models;

namespace Game.Contracts;

public class PveBattleViewModel
{
    public string Id { get; set; }
    public CombatPlayerViewModel CombatPlayer { get; set; }
    public Monster Monster { get; set; }
}
