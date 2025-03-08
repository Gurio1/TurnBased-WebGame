using Game.Backpack;
using Game.Core.Models;

namespace Game.Drop;

public interface IDropable : IBackpackItem
{
    public float DropChance { get; set; }

    public void HandleDrop(Hero hero);
}