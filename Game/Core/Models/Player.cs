using System.Text.Json.Serialization;
using Game.Core.Abilities;
using Game.Core.Equipment;
using Game.Core.SharedKernel;
using Game.Core.StatusEffects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Core.Models;

public class Player : IHasAbilityIds
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    public string? BattleId { get; set; }
    public Stats Stats { get; set; } = new();
    [BsonIgnore] public List<Ability> Abilities { get; set; } = [];
    public Dictionary<string, EquipmentBase?> Equipment { get; set; } = [];
    public List<InventorySlot> Inventory { get; set; } = [];
    public List<IDebuff> Debuffs { get; set; } = [];
    public string CharacterType { get; set; } = "Player";
    [JsonIgnore] public required List<string> AbilityIds { get; set; } = [];
    
    public ResultWithoutValue Equip(string equipmentId)
    {
        var item = Inventory
            .FirstOrDefault(s => s.Item.Id == equipmentId)?
            .Item;
        
        if (item is null)
            return ResultWithoutValue.NotFound($"Unable to retrieve item with id '{equipmentId}'");
        
        if (item is not EquipmentBase equipment || !item.CanInteract(ItemInteractions.Equip))
            return ResultWithoutValue.Invalid($"Item '{item.Name}' doesn't have equip behaviour.");
        
        if (Equipment.TryGetValue(equipment.Slot, out var equippedItem)
            && equippedItem is not null)
        {
            AddToInventory(equippedItem);
            equippedItem.RemoveStats(this);
        }
        
        Equipment[equipment.Slot] = equipment;
        RemoveItemFromInventory(equipment);
        equipment.ApplyStats(this);
        
        return ResultWithoutValue.Success();
    }
    
    public ResultWithoutValue Unequip(string equipmentSlot)
    {
        if (!Equipment.TryGetValue(equipmentSlot, out var equippedItem))
            return ResultWithoutValue.Invalid($"'{equipmentSlot} slot doesn't exist as equipment slot");
        
        if (equippedItem == null)
            return ResultWithoutValue.Invalid(
                $"Player with id '{Id}' doesn't have equipped item on slot '{equipmentSlot}'");
        
        equippedItem.RemoveStats(this);
        Equipment.Remove(equipmentSlot);
        AddToInventory(equippedItem);
        
        return ResultWithoutValue.Success();
    }
    
    public void RemoveUsedItems(Dictionary<string, int> usedItems)
    {
        foreach (var usedItem in usedItems)
        {
            var itemSlots = Inventory
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
                    RemoveSlotFromInventory(slot);
                }
            }
        }
    }
    
    public void AddToInventory(Item item)
    {
        var slot = Inventory.FirstOrDefault(i => i.Item.Id == item.Id && i.Quantity < item.MaxInventorySlotQuantity);
        
        if (slot is null)
            Inventory.Add(new InventorySlot { Item = item, Quantity = 1 });
        else
            slot.Quantity++;
    }
    
    //TODO : Add validation if slot item quantity <= 0
    public void RemoveSlotFromInventory(InventorySlot slot) => Inventory.Remove(slot);
    
    public void RemoveItemFromInventory(Item item)
    {
        var slot = Inventory.FirstOrDefault(s => s.Item.Id == item.Id);
        
        slot.Quantity--;
        
        if (slot.Quantity == 0)
            RemoveSlotFromInventory(slot);
    }
    
    public bool InBattle() => BattleId is not null;
}
