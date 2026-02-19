using Game.Core.Models;

namespace Game.Core.PlayerProfile.ValueObjects;

public class InventorySlot
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public Item Item { get; init; } = null!;
    public int Quantity { get; private set; }
    
    public bool IsFull => Quantity >= Item.MaxInventorySlotQuantity;
    public bool IsEmpty => Quantity == 0;
    private int RemainingCapacity => Item.MaxInventorySlotQuantity - Quantity;
    
    public int AddQuantity(int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(quantity);

        if (IsFull)
            return quantity;
        
        int toAdd = Math.Min(quantity, RemainingCapacity);
        Quantity += toAdd;
        return quantity - toAdd;
    }
    
    public int RemoveQuantity(int quantity)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(quantity);
        
        if (Quantity == 0)
            return quantity;
        
        int toRemove = Math.Min(quantity, Quantity);
        Quantity -= toRemove;
        
        int remainder = quantity - toRemove;
        return remainder;
    }
    
    
}
