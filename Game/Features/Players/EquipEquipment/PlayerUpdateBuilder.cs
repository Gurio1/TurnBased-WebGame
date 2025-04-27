using Game.Core.Models;
using MongoDB.Driver;

namespace Game.Features.Players.EquipEquipment;

public static class PlayerUpdateBuilder
{
    public static UpdateDefinition<Player> Build(Player player) =>
        Builders<Player>.Update
            .Set(p => p.Equipment, player.Equipment)
            .Set(p => p.Inventory, player.Inventory)
            .Set(p => p.Stats, player.Stats);
}
