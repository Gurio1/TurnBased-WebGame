using Game.Core;
using Game.Core.Models;

namespace Game.Drop;

internal class DropService : IDropService
{
    private readonly Random _rnd = new Random();
    
    public IDropable GiveDrop(IMonster monster)
    {
        foreach (var drop in monster.ListOfDrops)
        {
            var chance = _rnd.NextDouble();

            if (drop.DropChance > chance)
            {
                return drop;
            }
        }

        return null;
    }
}