using Game.Core.Equipment.Generation;
using Game.Core.Models;
using Game.SharedKernel.Results;

namespace Game.Core.Loot;

public class LootService : ILootService
{
    private readonly IEquipmentGenerator equipmentGenerator;
    
    public LootService(IEquipmentGenerator equipmentGenerator)
        => this.equipmentGenerator = equipmentGenerator;
    
    public async Task<Result<GeneratedMonsterDrop?>> GenerateDrop(Monster monster)
    {
        if (monster.Drops.Count == 0 || monster.OverallDropChance <= 0)
        {
            return Result<GeneratedMonsterDrop?>.Success(null);
        }

        if (Random.Shared.NextDouble() > monster.OverallDropChance)
        {
            return Result<GeneratedMonsterDrop?>.Success(null);
        }

        var eligibleDrops = monster.Drops
            .Where(drop => drop.Weight > 0)
            .ToList();

        if (eligibleDrops.Count == 0)
        {
            return Result<GeneratedMonsterDrop?>.Success(null);
        }

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

            if (!ItemCatalog.TryGetDefinition(drop.ItemTypeName, drop.ItemId, out var definition) || definition is null)
            {
                return Result<GeneratedMonsterDrop?>.Invalid($"Drop '{drop.ItemId}' is not part of the item catalog.");
            }

            if (definition.IsEquipment)
            {
                var equipmentResult = await equipmentGenerator.GenerateEquipment(drop.ItemTypeName);
                return equipmentResult.IsFailure
                    ? equipmentResult.AsError<GeneratedMonsterDrop?>()
                    : Result<GeneratedMonsterDrop?>.Success(new GeneratedMonsterDrop(equipmentResult.Value, drop.Quantity));
            }

            var itemResult = ItemCatalog.Create(drop.ItemTypeName);
            return itemResult.IsFailure
                ? itemResult.AsError<GeneratedMonsterDrop?>()
                : Result<GeneratedMonsterDrop?>.Success(new GeneratedMonsterDrop(itemResult.Value, drop.Quantity));
        }

        return Result<GeneratedMonsterDrop?>.Success(null);
    }
}
