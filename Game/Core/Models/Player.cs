using System.Text.Json.Serialization;
using Game.Core.Abilities;
using Game.Core.AbilityEffects;
using Game.Core.Equipment;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Core.Models;

public class Player
{
    [BsonElement("Inventory")]
    private List<InventorySlot> _inventory = new();
    
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    public string? BattleId { get; set; }
    public Stats Stats { get; set; } = new Stats();
    
    [JsonIgnore]
    public required List<string> AbilityIds { get; set; } = [];
  
    [BsonIgnore]
    public List<Ability> Abilities { get; set; } = [];
    public Dictionary<string, EquipmentBase?> Equipment { get; set; } = [];
    
    [BsonIgnore] 
    public IReadOnlyList<InventorySlot> Inventory => _inventory;
    public List<IDebuff> Debuffs { get; set; } = [];
    public string CharacterType { get; set; } = "Player";

    public void Equip(EquipmentBase equipmentItem)
    {
        if (Equipment.TryGetValue(equipmentItem.Slot, out var equippedItem) && equippedItem is not null)
        {
            AddToInventory(equippedItem);
            equippedItem.RemoveStats(this);
        }

        Equipment[equipmentItem.Slot] = equipmentItem;
        RemoveItemFromInventory(equipmentItem);
        equipmentItem.ApplyStats(this);
    }
    
    public bool Unequip(string equipmentSlot)
    {
        if (!Equipment.TryGetValue(equipmentSlot, out EquipmentBase? equippedItem)) return false;
        
        
        if (equippedItem is null)
        {
            return false;
        }
            
        equippedItem.RemoveStats(this);
        Equipment.Remove(equipmentSlot);
        AddToInventory(equippedItem);

        return true;

    }

    public void AddToInventory(Item item)
    {
        var slot = Inventory.FirstOrDefault(i => i.Item.Id == item.Id && i.Quantity < item.MaxInventorySlotQuantity);

        if (slot is null)
        {
            _inventory.Add(new InventorySlot(){Item = item,Quantity = 1});
        }
        else
        {
            slot.Quantity++;
        }
    }
    
    public void RemoveSlotFromInventory(InventorySlot slot)
    {
        _inventory.Remove(slot);
    }

    public void RemoveItemFromInventory(Item item)
    {
        var slot = Inventory.FirstOrDefault(s => s.Item.Id == item.Id);

        slot.Quantity--;

        if (slot.Quantity == 0)
        {
            _inventory.Remove(slot);
        }
    }
    
    public bool InBattle()
    {
        return BattleId is not null;
    }
}