using Game.Drop;

namespace Game.Core.Models;

public interface IMonster 
{
    public int Level { get; set; }
    public List<IDropable> ListOfDrops { get; set; }
}