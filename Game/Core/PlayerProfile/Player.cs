using System.Text.Json.Serialization;
using Game.Application.SharedKernel;
using Game.Core.Abilities;
using Game.Core.Equipment;
using Game.Core.Models;
using Game.Core.PlayerProfile.ValueObjects;
using Game.Core.StatusEffects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Core.PlayerProfile;

public class Player : IHasAbilityIds
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = ObjectId.GenerateNewId().ToString();
    
    public string? BattleId { get; private set; }
    public Stats Stats { get; set; } = new();
    [BsonIgnore] public List<Ability> Abilities { get; set; } = [];
    public Dictionary<string, EquipmentBase?> Equipment { get; init; } = [];
    public Inventory Inventory { get; init; } = new();
    public List<IDebuff> Debuffs { get; init; } = [];
    public string CharacterType { get; set; } = "Player";
    [JsonIgnore] public required List<string> AbilityIds { get; init; } = [];
    
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
            Inventory.Add(equippedItem);
            equippedItem.RemoveStats(this);
        }
        
        Equipment[equipment.Slot] = equipment;
        Inventory.RemoveItem(equipment);
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
        Inventory.Add(equippedItem);
        
        return ResultWithoutValue.Success();
    }
    
    public bool InBattle() => BattleId is not null;
    public void SetBattleId(string battleId) => BattleId = battleId;
    public void ResetBattleId() => BattleId = null;
}
