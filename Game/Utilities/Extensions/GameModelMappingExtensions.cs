using Game.Contracts;
using Game.Core.Equipment;
using Game.Core.Models;
using Game.Core.PlayerProfile.Aggregates;
using Game.Core.PlayerProfile.ValueObjects;
using Game.SharedKernel.Battle;

namespace Game.Utilities.Extensions;

public static class GameModelMappingExtensions
{
    #region Private methods
    private static ItemViewModel ToDto(this Item item) => item switch
    {
        EquipmentBase e => e.ToEquipmentDto(),
        _ => item.ToItemDto()
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
        new() { Item = slot.Item.ToDto().WithActionsFrom(slot.Item, urls), Quantity = slot.Quantity };

    #endregion

    public static PlayerViewModel ToViewModel(this GamePlayer model, UrlBuilder urlBuilder) =>
        new()
        {
            Id = model.Id,
            Stats = model.Stats,
            CharacterType = model.CharacterType,
            Equipment = model.Equipment
                .ToDictionary(
                    kvp => kvp.Key,
                    kvp => kvp.Value?.ToEquipmentDto()
                ),
            Inventory = model.Inventory.Select(i => i.ToInventorySlotDto(urlBuilder)).ToList(),
        };

    public static BattlePlayerViewModel ToBattleViewModel(this GamePlayer model) =>
        new()
        {
            Id = model.Id,
            Stats = model.Stats,
            AbilityIds = model.AbilityIds.ToList(),
            CharacterType = model.CharacterType
        };

    public static BattlePlayerSnapshot ToBattleSnapshot(this GamePlayer model) =>
        new()
        {
            Id = model.Id,
            Stats = model.Stats,
            AbilityIds = model.AbilityIds.ToList(),
            CharacterType = model.CharacterType
        };

    public static BattleMonsterSnapshot ToBattleSnapshot(this Monster model) =>
        new()
        {
            Name = model.Name,
            Stats = model.Stats,
            AbilityIds = model.AbilityIds.ToList(),
            OverallDropChance = model.OverallDropChance,
            Drops = model.Drops
                .Select(drop => new BattleMonsterDropSnapshot
                {
                    ItemTypeName = drop.ItemTypeName,
                    ItemId = drop.ItemId,
                    Quantity = drop.Quantity,
                    Weight = drop.Weight
                })
                .ToList()
        };
}
