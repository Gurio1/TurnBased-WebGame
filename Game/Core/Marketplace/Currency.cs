using MongoDB.Bson.Serialization.Attributes;

namespace Game.Core.Marketplace;

[BsonDiscriminator(Required = true)]
public abstract class Currency
{
    public abstract string Name { get;}
    public int Amount { get; set; }
}
