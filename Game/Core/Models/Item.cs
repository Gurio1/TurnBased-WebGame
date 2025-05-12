using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Core.Models;

[BsonDiscriminator(Required = true)]
public abstract class Item
{
    public string Id { get; set; } = Guid.CreateVersion7().ToString();
    public abstract string Name { get; set; }
    
    [BsonRepresentation(BsonType.String)] public abstract ItemType ItemType { get; set; }
    
    public abstract string ImageUrl { get; set; }
    
    [BsonRepresentation(BsonType.String)] public abstract ItemInteractions Interactions { get; }
    
    public abstract int MaxInventorySlotQuantity { get; set; }
    public bool CanInteract(ItemInteractions interaction) => (Interactions & interaction) == interaction;
}

//TODO : I dont think i need this enum. Can add SetIsRootClass to root classes for mongo 
public enum ItemType
{
    Equipment, // Weapons, Armor
    Consumable, // Potions, Scrolls
    Currency, // Gold, Gems
    EventItem, // Quest-related items
    Other // Future expansion (crafting, keys, etc.)
}

[Flags]
public enum ItemInteractions
{
    None = 0,
    Use = 1 << 0, // 1 (0001) -> Potions, Scrolls
    Equip = 1 << 1, // 2 (0010) -> Weapons, Armor
    Sell = 1 << 2, // 4 (0100) -> Most items
    Trade = 1 << 3 // 8 (1000) -> Tradable items
}
