using System.Collections;
using Game.Application.SharedKernel;
using Game.Core.Models;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Game.Core.PlayerProfile.ValueObjects;

public sealed class Inventory : IEnumerable<InventorySlot>
{
    [BsonElement("slots")] private readonly List<InventorySlot> inventorySlots;
    
    public IEnumerator<InventorySlot> GetEnumerator() => inventorySlots.GetEnumerator();
    
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    
    [BsonConstructor]
    [JsonConstructor] 
    public Inventory(IEnumerable<InventorySlot>? slots = null) =>
        inventorySlots = slots?.ToList() ?? new List<InventorySlot>();
    
    public Result<Item> GetItem(string itemId)
    {
        var item = inventorySlots
            .FirstOrDefault(s => s.Item.Id == itemId)?
            .Item;
        
        return item is null 
            ? Result<Item>.NotFound($"Unable to retrieve item with id '{itemId}'") 
            : Result<Item>.Success(item);
    }
    public void Add(Item item, int quantity)
    {
        int remainingQuantity = quantity;
        
        while (remainingQuantity > 0)
        {
            var slot = inventorySlots.FirstOrDefault(i => i.Item.Id == item.Id && !i.IsFull);
            
            if (slot is null)
            {
                slot = new InventorySlot { Item = item };
                inventorySlots.Add(slot);
            }
            
            remainingQuantity = slot.AddQuantity(remainingQuantity);
        }
    }
    public void RemoveUsedItems(Dictionary<string, int> usedItems)
    {
        foreach (var usedItem in usedItems)
        {
            var itemSlots = inventorySlots
                .Where(i => i.Item.Id == usedItem.Key)
                .ToArray();
            
            int remainingQuantity = usedItem.Value;
            
            foreach (var slot in itemSlots)
            {
                ArgumentOutOfRangeException.ThrowIfNegative(remainingQuantity);
                
                if (remainingQuantity == 0)
                    break;
                
                remainingQuantity = slot.RemoveQuantity(remainingQuantity);
                
                if (slot.IsEmpty)
                {
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
        
        slot.RemoveQuantity(1);
        
        if (slot.IsEmpty)
            RemoveSlot(slot);
    }
    private void RemoveSlot(InventorySlot slot) => inventorySlots.Remove(slot);
}
