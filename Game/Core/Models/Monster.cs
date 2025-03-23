using Game.Core.Abilities;
using Game.Core.AbilityEffects;
using Game.Core.Equipment;

namespace Game.Core.Models;

public abstract class Monster : CharacterBase,IMonster
{
    public abstract Dictionary<string,float> DropsTable { get; init; }
}