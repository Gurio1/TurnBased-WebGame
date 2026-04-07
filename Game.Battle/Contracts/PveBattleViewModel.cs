using Game.Battle.Core.Battle.PVE;
using Game.Battle.Core.Models;

namespace Game.Battle.Contracts;

public class PveBattleViewModel(PveBattle battle)
{
    public string Id { get; set; } = battle.Id;
    public CombatPlayerViewModel CombatPlayer { get; set; } = new(battle.Player);
    public Monster Monster { get; set; } = battle.Monster;
}
