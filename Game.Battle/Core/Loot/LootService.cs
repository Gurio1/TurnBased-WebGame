using Game.Battle.Core.Models;
using Game.SharedKernel.Results;

namespace Game.Battle.Core.Loot;

public sealed class LootService : ILootService
{
    public Task<Result<LootResult?>> GenerateDrop(Monster monster)
    {
        if (monster.Drops.Count == 0 || monster.OverallDropChance <= 0)
            return Task.FromResult(Result<LootResult?>.Success(null));

        if (Random.Shared.NextDouble() > monster.OverallDropChance)
            return Task.FromResult(Result<LootResult?>.Success(null));

        var eligibleDrops = monster.Drops
            .Where(drop => drop.Weight > 0)
            .ToList();

        if (eligibleDrops.Count == 0)
            return Task.FromResult(Result<LootResult?>.Success(null));

        var totalWeight = eligibleDrops.Sum(drop => drop.Weight);
        var roll = Random.Shared.NextDouble() * totalWeight;
        var cumulativeWeight = 0d;

        foreach (var drop in eligibleDrops)
        {
            cumulativeWeight += drop.Weight;
            if (roll > cumulativeWeight)
            {
                continue;
            }

            Item item = drop.ItemTypeName.Contains("Wooden", StringComparison.OrdinalIgnoreCase)
                ? new Equipment
                {
                    Name = drop.ItemId,
                    ImageUrl = string.Empty,
                    Slot = "loot",
                    Quantity = drop.Quantity
                }
                : new LootItem
                {
                    Id = drop.ItemId,
                    Name = drop.ItemId,
                    ImageUrl = string.Empty,
                    Quantity = drop.Quantity
                };

            return Task.FromResult(Result<LootResult?>.Success(new LootResult(item, drop.Quantity)));
        }

        return Task.FromResult(Result<LootResult?>.Success(null));
    }
}
