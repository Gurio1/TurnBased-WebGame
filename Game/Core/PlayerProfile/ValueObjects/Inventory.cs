using System.Collections;
using Game.Core.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Core.PlayerProfile.ValueObjects;

public sealed class Inventory : IEnumerable<InventorySlot>
{
    [BsonElement("slots")]
    private readonly List<InventorySlot> inventorySlots = [];
    public IEnumerator<InventorySlot> GetEnumerator() => inventorySlots.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public void RemoveUsedItems(Dictionary<string, int> usedItems)
    {
        foreach (var usedItem in usedItems)
        {
            var itemSlots = inventorySlots
                .Where(i => i.Item.Id == usedItem.Key)
                .ToArray();
            
            int remainingToUse = usedItem.Value;
            
            foreach (var slot in itemSlots)
            {
                if (remainingToUse <= 0)
                    break;
                
                if (slot.Quantity > remainingToUse)
                {
                    slot.Quantity -= remainingToUse;
                    remainingToUse = 0;
                }
                else
                {
                    remainingToUse -= slot.Quantity;
                    RemoveSlot(slot);
                }
            }
        }
    }
    
    public void Add(Item item)
    {
        var slot = inventorySlots.FirstOrDefault(i => i.Item.Id == item.Id && i.Quantity < item.MaxInventorySlotQuantity);
        
        if (slot is null)
            inventorySlots.Add(new InventorySlot { Item = item, Quantity = 1 });
        else
            slot.Quantity++;
    }
    private void RemoveSlot(InventorySlot slot) => inventorySlots.Remove(slot);
    
    public void RemoveItem(Item item)
    {
        var slot = inventorySlots.FirstOrDefault(s => s.Item.Id == item.Id);
        
        if (slot is null)
        {
            return;
        }
        
        slot.Quantity--;
        
        if (slot.Quantity == 0)
            RemoveSlot(slot);
    }
}
