using MongoDB.Bson.Serialization.Attributes;

namespace Game.Core.Models;

[BsonDiscriminator(Required = true)]
public abstract class Item
{
    public virtual string Id { get; set; } = Guid.CreateVersion7().ToString();
    public abstract string Name { get; set; }
    public  abstract string ImageUrl { get; init; }
    public abstract int MaxInventorySlotQuantity { get; protected set; }
}
