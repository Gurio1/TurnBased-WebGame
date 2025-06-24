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
    public List<Currency> Currencies { get; set; } = CurrencyInitializer.InitializeAllCurrencies();
    public string? BattleId { get; private set; }
    public Stats Stats { get; set; } = new();
    [BsonIgnore] public List<Ability> Abilities { get; set; } = [];
    public Dictionary<string, EquipmentBase?> Equipment { get; init; } = [];
    public Inventory Inventory { get; init; } = [];
    public List<IDebuff> Debuffs { get; init; } = [];
    public string CharacterType { get; set; } = "Player";
    [JsonIgnore] public List<string> AbilityIds { get; init; } = [];
    
    public ResultWithoutValue Equip(string itemId)
    {
        var itemResult = Inventory.GetItem(itemId);
        
        if (itemResult.IsFailure)
        {
            return ResultWithoutValue.CreateError(itemResult.Error);
        }
        
        var item = itemResult.Value;
        
        if (item is not EquipmentBase equipment || !item.CanInteract(ItemInteractions.Equip))
            return ResultWithoutValue.Invalid($"Item '{item.Name}' doesn't have equip behaviour.");
        
        var spec = new EquipmentAction();
        var result = spec.IsSatisfiedBy(this);
        
        if (result.IsFailure)
        {
            return result;
        }
        
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
        
        //TODO: I dont know,for now this looks like business rule and validation above is technical implementation,so i will leave them separate.
        var spec = new EquipmentAction();
        var result = spec.IsSatisfiedBy(this);
        
        if (result.IsFailure)
        {
            return result;
        }
        
        equippedItem.RemoveStats(this);
        Equipment.Remove(equipmentSlot);
        Inventory.Add(equippedItem);
        
        return ResultWithoutValue.Success();
    }
    
    public Result<Currency> Sell(string itemId)
    {
        var itemResult = Inventory.GetItem(itemId);
        
        if (itemResult.IsFailure)
        {
            return itemResult.AsError<Currency>();
        }
        
        var item = itemResult.Value;
        
        if (!item.CanInteract(ItemInteractions.Sell))
            return Result<Currency>.Invalid($"Item '{item.Name}' doesn't have sell behaviour.");
        
        var actualCurrency = Currencies.FirstOrDefault(x => x.Name == item.SellPrice.Name);
        
        actualCurrency!.Amount += item.SellPrice.Amount;
        
        Inventory.RemoveItem(item);
        
        return Result<Currency>.Success(actualCurrency);
    }
    
    public bool InBattle() => BattleId is not null;
    public void ResetBattleId() => BattleId = null;
}
