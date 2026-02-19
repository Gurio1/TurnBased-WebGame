using Game.Core.PlayerProfile.ValueObjects;

namespace Game.Utilities.Extensions;

public static class EnumerableExtensions
{
    public static (List<TInterface> matches, List<InventorySlot> others)
        PartitionBy<TInterface>(this IEnumerable<InventorySlot> source)
        where TInterface : class
    {
        var matches = new List<TInterface>();
        var others  = new List<InventorySlot>();
        foreach (var x in source)
        {
            if (x.Item is TInterface t)
                matches.Add(t);
            else
                others.Add(x);
        }
        return (matches, others);
    }
}
