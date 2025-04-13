namespace Game.Core.Models;

public class InventorySlot
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public Item Item { get; init; }
    public int Quantity { get; set; }
}
