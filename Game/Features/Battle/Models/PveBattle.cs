using Game.Core.Models;

namespace Game.Features.Battle.Models;

public class PveBattle
{
    public PveBattle() { }
    
    public PveBattle(CombatPlayer player, Monster monster)
    {
        Monster = monster;
        CombatPlayer = player;
    }
    
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public CombatPlayer CombatPlayer { get; set; }
    public Monster Monster { get; set; }
    
    public void UpdatePlayer(Player player) =>
        CombatPlayer = player.ToPlayerBattleModel(CombatPlayer.UsedItems);
}
