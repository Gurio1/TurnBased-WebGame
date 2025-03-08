using Game.Core.Models;

namespace Game.Backpack;
public interface IBackpackItem : IItem
{
    int CountOfOneStack { get; }
}