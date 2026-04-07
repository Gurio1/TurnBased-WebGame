namespace Game.Core.Models;

public sealed record ItemCatalogEntry(
    string TypeName,
    string ItemId,
    string Name,
    string ImageUrl,
    string Category,
    bool IsEquipment);
