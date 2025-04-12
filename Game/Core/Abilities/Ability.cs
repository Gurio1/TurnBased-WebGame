using Game.Core.Models;
using Game.Features;
using Game.Features.Battle.Models;
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
    public abstract string ImageUrl { get; set; }
    public abstract int Cooldown  { get; init; }
    public abstract int CurrentCooldown { get; set; }
    
    public abstract void Execute(CombatEntity owner, CombatEntity target, BattleContext context);
    public abstract string GetAbilityDescription(Player player);

    public void DecreaseCurrentCooldown()
    {
        if (CurrentCooldown != 0)
        {
            CurrentCooldown--;
        }
    }
}