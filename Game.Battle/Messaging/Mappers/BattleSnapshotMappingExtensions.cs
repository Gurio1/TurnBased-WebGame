using Game.Battle.Core.Models;
using Game.SharedKernel.Battle;

namespace Game.Battle.Messaging.Mappers;

public static class BattleSnapshotMappingExtensions
{
    public static Player ToBattlePlayer(this BattlePlayerSnapshot snapshot) =>
        new()
        {
            Id = snapshot.Id,
            Stats = snapshot.Stats,
            AbilityIds = snapshot.AbilityIds.ToList(),
            BattleId = snapshot.BattleId,
            CharacterType = snapshot.CharacterType
        };

    public static Monster ToBattleMonster(this BattleMonsterSnapshot snapshot) =>
        new()
        {
            Id = string.Empty,
            Name = snapshot.Name,
            Stats = snapshot.Stats,
            AbilityIds = snapshot.AbilityIds.ToList(),
            OverallDropChance = snapshot.OverallDropChance,
            Drops = snapshot.Drops
                .Select(drop => new MonsterDropEntry
                {
                    ItemTypeName = drop.ItemTypeName,
                    ItemId = drop.ItemId,
                    Quantity = drop.Quantity,
                    Weight = drop.Weight
                })
                .ToList()
        };
}
