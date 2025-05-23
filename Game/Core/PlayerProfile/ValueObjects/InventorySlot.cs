using Game.Core.Models;

namespace Game.Core.PlayerProfile.ValueObjects;

public class InventorySlot
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public Item Item { get; init; } = null!;
    public int Quantity { get; set; }
}
