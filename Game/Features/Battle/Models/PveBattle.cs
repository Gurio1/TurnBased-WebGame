using Game.Core.Models;

namespace Game.Features.Battle.Models;

public class PveBattle
{
    public PveBattle() { }
    
    public PveBattle(Player player, Monster monster, Dictionary<string, int> playerUsedItems)
    {
        Monster = monster;
        CombatPlayer = player.ToPlayerBattleModel(playerUsedItems);
    }
    
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public CombatPlayer CombatPlayer { get; set; }
    public Monster Monster { get; set; }
    
    public void UpdatePlayer(Player player) =>
        CombatPlayer = player.ToPlayerBattleModel(CombatPlayer.UsedItems);
}
