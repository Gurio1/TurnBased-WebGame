using Game.Core.Models;

namespace Game.Drop;

internal interface IDropService
{
    public IDropable GiveDrop(IMonster monster);
}