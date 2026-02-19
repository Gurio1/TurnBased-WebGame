using Game.Core.Models;

namespace Game.Core.Loot;

public sealed class LootTable
{
    private readonly List<LootEntry> _entries = [];
    
    public LootTable Add(LootEntry lootEntry)
    {
        _entries.Add(lootEntry);
        return this;
    }
    
    public LootEntry? GetRandomDrop()
    {
        if (_entries.Count == 0)
            throw new InvalidOperationException("Loot table is empty.");
        
        double totalChance = _entries.Sum(x => x.Chance);
        double roll = Random.Shared.NextDouble() * totalChance;
        double cumulative = 0;
        
        foreach (var entry in _entries)
        {
            cumulative += entry.Chance;
            if (roll <= cumulative)
                return entry;
        }
        
        return null;
    }
    
    public IEnumerable<Item> GetAllEntries() => _entries.Select(e => e.Item);
}

