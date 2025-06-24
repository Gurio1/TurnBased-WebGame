using System.Collections;
using Game.Application.SharedKernel;
using Game.Core.Models;
using MongoDB.Bson.Serialization.Attributes;
using NetTopologySuite.Index.HPRtree;

namespace Game.Core.PlayerProfile.ValueObjects;

public sealed class Inventory : IEnumerable<InventorySlot>
{
    [BsonElement("slots")] private readonly List<InventorySlot> inventorySlots = [];
    
    public void Add(InventorySlot slot)
        => inventorySlots.Add(slot);
    public IEnumerator<InventorySlot> GetEnumerator() => inventorySlots.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    public Result<Item> GetItem(string itemId)
    {
        var item = inventorySlots
            .FirstOrDefault(s => s.Item.Id == itemId)?
            .Item;
        
        return item is null 
            ? Result<Item>.NotFound($"Unable to retrieve item with id '{itemId}'") 
            : Result<Item>.Success(item);
    }
    public void Add(Item item)
    {
        var slot = inventorySlots.FirstOrDefault(i => i.Item.Id == item.Id && i.Quantity < item.MaxInventorySlotQuantity);
        
        if (slot is null)
            inventorySlots.Add(new InventorySlot { Item = item, Quantity = 1 });
        else
            slot.Quantity++;
    }
    
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
    
    private void RemoveSlot(InventorySlot slot) => inventorySlots.Remove(slot);
}
