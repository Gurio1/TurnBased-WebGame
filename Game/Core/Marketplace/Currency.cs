using Game.Core.Models;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Core.Marketplace;

[BsonDiscriminator(Required = true)]
public abstract class Currency : Item
{
    public int Quantity { get; set; }
}
