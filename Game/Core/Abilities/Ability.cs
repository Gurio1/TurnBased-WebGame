using Game.Core.Models;
using MediatR;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;

namespace Game.Core.Abilities;

[BsonDiscriminator(Required = true)]
public abstract class Ability 
{
    public abstract string TypeName { get; init; }
    public abstract string Id { get; set; } 
    public abstract string Name { get; set; }
    public abstract int Cooldown  { get; init; }
    public abstract int CurrentCooldown { get; set; }
    
    public abstract float Execute(CharacterBase owner, CharacterBase target, IMediator mediator);

    public void DecreaseCurrentCooldown()
    {
        if (CurrentCooldown != 0)
        {
            CurrentCooldown--;
        }
    }
}