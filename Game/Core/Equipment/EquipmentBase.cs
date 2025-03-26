using Game.Core.Models;
using Game.Features.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Core.Equipment;

[BsonDiscriminator(Required = true)]
public abstract class EquipmentBase : Item
{
    public abstract string EquipmentId { get; set; }
    public override ItemType ItemType { get; set; } = ItemType.Equipment;
    public abstract string Slot { get; set; }
    public override ItemInteractions Interactions { get; } = ItemInteractions.Equip | ItemInteractions.Sell;
    public List<EquipmentAttribute> Attributes { get; set; } = new();

    public void ApplyStats(CharacterBase characterBase)
    {
        Attributes.ForEach(a => a.ApplyStats(characterBase));
    }

    public void RemoveStats(CharacterBase characterBase)
    {
        Attributes.ForEach(a => a.RemoveStats(characterBase));
    }
    
}   