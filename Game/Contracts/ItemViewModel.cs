using System.Text.Json.Serialization;
using Game.Core.Equipment;
using Game.Core.Marketplace;
using Game.Core.Models;
using Game.Core.PlayerProfile.ValueObjects;
using Game.Utilities;
using Game.Utilities.Extensions;

namespace Game.Contracts;

public sealed class ItemActionDto
{
    public string Key { get; init; } = string.Empty;
    public string Href { get; init; } = string.Empty;
    public string? Method { get; init; }
}

[JsonPolymorphic(TypeDiscriminatorPropertyName = nameof(Type))]
[JsonDerivedType(typeof(EquipmentViewModel), nameof(EquipmentBase))]
public class ItemViewModel
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public required string Type { get; set; }
    public required string ImageUrl { get; init; }
    // ReSharper disable once CollectionNeverQueried.Global
    public List<ItemActionDto> ItemActions { get; init; } = [];
}

public sealed class EquipmentViewModel : ItemViewModel
{
    public required string EquipmentId { get; init; }
    public required Currency SellPrice { get; init; }
    public required string Slot { get; init; }
    public List<EquipmentStat> Attributes { get; init; } = [];
}

public sealed class InventorySlotViewModel
{
    public required ItemViewModel Item { get; init; }
    public int Quantity { get; init; }
}
