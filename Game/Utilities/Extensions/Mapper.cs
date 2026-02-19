using Game.Contracts;
using Game.Core.Abilities;
using Game.Core.Battle;
using Game.Core.Battle.PVE;
using Game.Core.Equipment;
using Game.Core.Models;
using Game.Core.PlayerProfile.Aggregates;
using Game.Core.PlayerProfile.ValueObjects;

namespace Game.Utilities.Extensions;

public static class Mapper
{
    #region Private methods
    private static CombatPlayerViewModel ToViewModel(this CombatPlayer model) =>
        new()
        {
            Id = model.Id,
            Stats = model.Stats,
            Abilities = model.Abilities,
            Debuffs = model.Debuffs,
            CharacterType = model.CharacterType,
            Equipment = model.Equipment,
            OtherInventoryItems = model.OtherInventoryItems
        };
    
    private static AbilityHomeViewModel ToAbilityHomeViewModel(this Ability ability, GamePlayer abilityOwner) =>
        new()
        {
            Id = ability.Id,
            Name = ability.Name,
            Cooldown = ability.Cooldown,
            Description = ability.GetAbilityDescription(abilityOwner)
        };
    
    private static ItemViewModel ToDto(this Item item) => item switch
    {
        EquipmentBase e => e.ToEquipmentDto(),
        _  => item.ToItemDto()
    };
    private static ItemViewModel ToItemDto(this Item item) =>
        new()
        {
            Id = item.Id,
            Type = item.GetType().Name,
            Name = item.Name,
            ImageUrl = item.ImageUrl
        };
    
    private static EquipmentViewModel ToEquipmentDto(this EquipmentBase e)
    {
        var baseDto = e.ToItemDto();
        return new EquipmentViewModel
        {
            Id = baseDto.Id,
            Name = baseDto.Name,
            ImageUrl = baseDto.ImageUrl,
            Type = "equipment",
            EquipmentId = e.EquipmentId,
            Slot = e.Slot,
            SellPrice = e.SellPrice,
            Attributes = e.Attributes
        };
    }
    
    private static InventorySlotViewModel ToInventorySlotDto(this InventorySlot slot, UrlBuilder urls) =>
        new() { Item = slot.Item.ToDto().WithActionsFrom(slot.Item,urls), Quantity = slot.Quantity };
    
    #endregion
    public static PlayerViewModel ToViewModel(this GamePlayer model, UrlBuilder urlBuilder) =>
        new()
        {
            Id = model.Id,
            Stats = model.Stats,
            Abilities = model.Abilities.Select(a => a.ToAbilityHomeViewModel(model)).ToList(),
            Debuffs = model.Debuffs,
            CharacterType = model.CharacterType,
            Equipment =  model.Equipment
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.ToEquipmentDto()
                ),
            Inventory = model.Inventory.Select(i => i.ToInventorySlotDto(urlBuilder)).ToList(),
        };
    
    public static PveBattleViewModel ToViewModel(this PveBattle battle) =>
        new() { Id = battle.Id, CombatPlayer = battle.CombatPlayer.ToViewModel(), Monster = battle.Monster };
}
