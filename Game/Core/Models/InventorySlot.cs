namespace Game.Core.Models;

public class InventorySlot
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public Item Item { get; set; }
    public int Quantity { get; set; }
}