using Game.Core.Abilities;
using Game.Core.AbilityEffects;
using Game.Core.Equipment;
using Game.Drop;

namespace Game.Core.Models;

public abstract class Monster : CharacterBase,IMonster
{
    public abstract int Level { get; set; }
    public abstract List<IDropable> ListOfDrops { get; set; }
}