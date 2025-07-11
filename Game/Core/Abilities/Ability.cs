using Game.Core.Battle;
using Game.Core.PlayerProfile;
using Game.Core.PlayerProfile.Aggregates;
using MongoDB.Bson.Serialization.Attributes;

namespace Game.Core.Abilities;

[BsonDiscriminator(Required = true)]
public abstract class Ability
{
    public abstract string TypeName { get; init; }
    public abstract string Id { get; set; }
    public abstract string Name { get; set; }
    public abstract string ImageUrl { get; set; }
    public abstract int Cooldown { get; init; }
    public abstract int CurrentCooldown { get; protected set; }
    
    public abstract void Execute(CombatEntity owner, CombatEntity target, BattleContext context);
    public abstract string GetAbilityDescription(GamePlayer gamePlayer);
    
    public void DecreaseCurrentCooldown()
    {
        if (CurrentCooldown != 0) CurrentCooldown--;
    }
}
