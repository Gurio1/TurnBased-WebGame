using System.Text.Json.Serialization;
using Game.Application;
using Game.Application.SharedKernel;
using Game.Core.Abilities;
using Game.Core.Equipment;
using Game.Core.Marketplace;
using Game.Core.Models;
using Game.Core.PlayerProfile.Specifications;
using Game.Core.PlayerProfile.ValueObjects;
using Game.Core.StatusEffects;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Core.PlayerProfile.Aggregates;

public class GamePlayer : IHasAbilityIds
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; init; } = ObjectId.GenerateNewId().ToString();
    
    public string? BattleId { get; private set; }
    public Stats Stats { get; set; } = new();
    [BsonIgnore] public List<Ability> Abilities { get; set; } = [];
    public Dictionary<string, EquipmentBase?> Equipment { get; init; } = [];
    public Inventory Inventory { get; init; } = new();
    public List<IDebuff> Debuffs { get; init; } = [];
    public string CharacterType { get; set; } = "Player";
    [JsonIgnore] public List<string> AbilityIds { get; init; } = [];
    
    public bool InBattle() => BattleId is not null;
    public void ResetBattleId() => BattleId = null;
    
    public ResultWithoutValue Equip(string itemId)
    {
        var itemResult = Inventory.GetItem(itemId);
        
        if (itemResult.IsFailure)
        {
            return ResultWithoutValue.CreateError(itemResult.Error);
        }
        
        var item = itemResult.Value;
        
        if (item is not EquipmentBase equipment)
            return ResultWithoutValue.Invalid($"Item '{item.Name}' is not equipment.");
        
        var spec = new EquipmentAction();
        var result = spec.IsSatisfiedBy(this);
        
        if (result.IsFailure)
        {
            return result;
        }
        
        if (Equipment.TryGetValue(equipment.Slot, out var equippedItem)
            && equippedItem is not null)
        {
            Inventory.Add(equippedItem, 1);
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
            return ResultWithoutValue.Invalid($"{equipmentSlot} slot doesn't exist as equipment slot");
        
        if (equippedItem == null)
            return ResultWithoutValue.Invalid(
                $"Player with id '{Id}' doesn't have equipped item on slot '{equipmentSlot}'");
        
        //TODO: I dont know,for now this looks like business rule and validation above is technical implementation,so i will leave them separate.
        var spec = new EquipmentAction();
        var result = spec.IsSatisfiedBy(this);
        
        if (result.IsFailure)
        {
            return result;
        }
        
        equippedItem.RemoveStats(this);
        Equipment.Remove(equipmentSlot);
        Inventory.Add(equippedItem, 1);
        
        return ResultWithoutValue.Success();
    }
    public ResultWithoutValue Sell(string itemId)
    {
        var itemResult = Inventory.GetItem(itemId);
        
        if (itemResult.IsFailure)
        {
            return itemResult.AsError<Currency>();
        }
        
        var item = itemResult.Value;
        
        if (item is not ISellable sellableItem)
            return ResultWithoutValue.Invalid($"Item '{item.Name}' can not be sold.");
        
        Inventory.Add(sellableItem.SellPrice, sellableItem.SellPrice.Quantity);
        
        Inventory.RemoveItem(item);
        
        return ResultWithoutValue.Success();
    }
}
