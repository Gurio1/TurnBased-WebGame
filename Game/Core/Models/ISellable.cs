using Game.Core.Marketplace;

namespace Game.Core.Models;

public interface ISellable
{
    public  Currency SellPrice { get; set; }

}
