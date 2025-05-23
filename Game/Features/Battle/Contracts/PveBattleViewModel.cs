using Game.Core.Battle.PVE;
using Game.Core.Models;

namespace Game.Features.Battle.Contracts;

public class PveBattleViewModel
{
    public string Id { get; set; }
    public CombatPlayerViewModel CombatPlayer { get; set; }
    public Monster Monster { get; set; }
}

public static partial class Mapper
{
    public static PveBattleViewModel ToViewModel(this PveBattle battle) =>
        new() { Id = battle.Id, CombatPlayer = battle.CombatPlayer.ToViewModel(), Monster = battle.Monster };
}
