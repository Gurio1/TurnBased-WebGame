using System.Reflection;
using Game.Core.Models;
using Game.SharedKernel.Models;
using Game.SharedKernel.Results;

namespace Game.Application.Monsters;

public sealed class MonsterService(IMonsterRepository monsterRepository) : IMonsterService
{
    private static readonly IReadOnlyList<MonsterAbilityCatalogEntry> AbilityCatalog =
    [
        new("0", "BaseAttack", "Base Attack", "BaseAttack.png", 0),
        new("1", "BleedAbility", "Bleed", "BleedSlash.png", 4),
        new("2", "SleepAbility", "Sleep", string.Empty, 4)
    ];

    private static readonly IReadOnlyList<MonsterStatCatalogEntry> StatCatalog = typeof(Stats)
        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .Select(property => new MonsterStatCatalogEntry(
            ToCamelCase(property.Name),
            SplitPascalCase(property.Name),
            property.PropertyType == typeof(int) ? "integer" : "decimal",
            0))
        .OrderBy(item => GetStatOrder(item.Key))
        .ToList();

    private static readonly Dictionary<string, PropertyInfo> StatProperties = typeof(Stats)
        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .ToDictionary(property => ToCamelCase(property.Name), StringComparer.OrdinalIgnoreCase);

    public Task<Result<IReadOnlyCollection<Monster>>> GetAll(CancellationToken ct = default) =>
        monsterRepository.GetAll(ct);

    public Task<Result<Monster>> GetByName(string monsterName, CancellationToken ct = default) =>
        monsterRepository.GetByName(monsterName, ct);

    public Task<Result<IReadOnlyCollection<ItemCatalogEntry>>> GetItemCatalog(CancellationToken ct = default) =>
        Task.FromResult(Result<IReadOnlyCollection<ItemCatalogEntry>>.Success(ItemCatalog.GetDefinitions()));

    public Task<Result<IReadOnlyCollection<MonsterAbilityCatalogEntry>>> GetAbilityCatalog(CancellationToken ct = default) =>
        Task.FromResult(Result<IReadOnlyCollection<MonsterAbilityCatalogEntry>>.Success(AbilityCatalog));

    public Task<Result<IReadOnlyCollection<MonsterStatCatalogEntry>>> GetStatCatalog(CancellationToken ct = default) =>
        Task.FromResult(Result<IReadOnlyCollection<MonsterStatCatalogEntry>>.Success(StatCatalog));

    public async Task<Result<Monster>> Create(SaveMonsterModel model, CancellationToken ct = default)
    {
        var validation = Validate(model);
        if (validation.IsFailure)
        {
            return Result<Monster>.CustomError(validation.Error);
        }

        if (await monsterRepository.ExistsByName(model.Name, ct: ct))
        {
            return Result<Monster>.Invalid($"Monster '{model.Name}' already exists.");
        }

        var monster = BuildMonster(model);
        return await monsterRepository.Create(monster, ct);
    }

    public async Task<ResultWithoutValue> Update(string currentMonsterName, SaveMonsterModel model, CancellationToken ct = default)
    {
        var validation = Validate(model);
        if (validation.IsFailure)
        {
            return validation;
        }

        var existingMonsterResult = await monsterRepository.GetByName(currentMonsterName, ct);
        if (existingMonsterResult.IsFailure)
        {
            return ResultWithoutValue.CreateError(existingMonsterResult.Error);
        }

        if (await monsterRepository.ExistsByName(model.Name, currentMonsterName, ct))
        {
            return ResultWithoutValue.Invalid($"Monster '{model.Name}' already exists.");
        }

        var monster = BuildMonster(model);
        monster.Id = existingMonsterResult.Value.Id;
        return await monsterRepository.Update(currentMonsterName, monster, ct);
    }

    public Task<ResultWithoutValue> Delete(string monsterName, CancellationToken ct = default) =>
        monsterRepository.Delete(monsterName, ct);

    private static ResultWithoutValue Validate(SaveMonsterModel model)
    {
        if (string.IsNullOrWhiteSpace(model.Name))
        {
            return ResultWithoutValue.Invalid("Monster name is required.");
        }

        if (model.OverallDropChance is < 0 or > 1)
        {
            return ResultWithoutValue.Invalid("Overall drop chance must stay between 0 and 1.");
        }

        var duplicateStatKeys = model.Stats
            .GroupBy(item => item.Key, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault(group => group.Count() > 1);

        if (duplicateStatKeys is not null)
        {
            return ResultWithoutValue.Invalid($"Stat '{duplicateStatKeys.Key}' can only be selected once.");
        }

        foreach (var stat in model.Stats)
        {
            if (!StatProperties.ContainsKey(stat.Key))
            {
                return ResultWithoutValue.Invalid($"Stat '{stat.Key}' is not supported.");
            }
        }

        var duplicateAbilityIds = model.AbilityIds
            .GroupBy(id => id, StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault(group => group.Count() > 1);

        if (duplicateAbilityIds is not null)
        {
            return ResultWithoutValue.Invalid($"Ability '{duplicateAbilityIds.Key}' can only be attached once.");
        }

        foreach (var abilityId in model.AbilityIds)
        {
            if (AbilityCatalog.All(item => !item.Id.Equals(abilityId, StringComparison.OrdinalIgnoreCase)))
            {
                return ResultWithoutValue.Invalid($"Ability '{abilityId}' is not supported.");
            }
        }

        var duplicateDrop = model.Drops
            .GroupBy(
                item => $"{item.ItemTypeName}::{item.ItemId}",
                StringComparer.OrdinalIgnoreCase)
            .FirstOrDefault(group => group.Count() > 1);

        if (duplicateDrop is not null)
        {
            return ResultWithoutValue.Invalid("Each drop item can only be attached once.");
        }

        foreach (var drop in model.Drops)
        {
            if (drop.Quantity <= 0)
            {
                return ResultWithoutValue.Invalid("Drop quantity must be greater than zero.");
            }

            if (drop.Weight < 0)
            {
                return ResultWithoutValue.Invalid("Drop weight must stay non-negative.");
            }

            if (!ItemCatalog.TryGetDefinition(drop.ItemTypeName, drop.ItemId, out _))
            {
                return ResultWithoutValue.Invalid($"Drop '{drop.ItemId}' is not part of the item catalog.");
            }
        }

        if (model.OverallDropChance > 0 && model.Drops.Count == 0)
        {
            return ResultWithoutValue.Invalid("Add at least one drop entry when overall drop chance is greater than zero.");
        }

        if (model.Drops.Count > 0 && model.Drops.All(item => item.Weight <= 0))
        {
            return ResultWithoutValue.Invalid("At least one drop entry must have a weight greater than zero.");
        }

        return ResultWithoutValue.Success();
    }

    private static Monster BuildMonster(SaveMonsterModel model)
    {
        var stats = new Stats();
        foreach (var stat in model.Stats)
        {
            ApplyStat(stats, stat);
        }

        if (!model.Stats.Any(item => item.Key.Equals("currentHealth", StringComparison.OrdinalIgnoreCase)) &&
            model.Stats.FirstOrDefault(item => item.Key.Equals("maxHealth", StringComparison.OrdinalIgnoreCase)) is { } maxHealth)
        {
            ApplyStat(stats, new MonsterStatValueModel("currentHealth", maxHealth.Value));
        }

        return new Monster
        {
            Name = model.Name.Trim(),
            Stats = stats,
            OverallDropChance = model.OverallDropChance,
            Drops = model.Drops
                .Select(drop => new MonsterDropEntry
                {
                    ItemTypeName = drop.ItemTypeName,
                    ItemId = drop.ItemId,
                    Quantity = drop.Quantity,
                    Weight = drop.Weight
                })
                .ToList(),
            AbilityIds = model.AbilityIds.ToList()
        };
    }

    private static void ApplyStat(Stats stats, MonsterStatValueModel stat)
    {
        var property = StatProperties[stat.Key];

        if (property.PropertyType == typeof(int))
        {
            property.SetValue(stats, Convert.ToInt32(Math.Round(stat.Value, MidpointRounding.AwayFromZero)));
            return;
        }

        property.SetValue(stats, Convert.ToSingle(stat.Value));
    }

    private static string ToCamelCase(string value) =>
        string.IsNullOrEmpty(value)
            ? value
            : $"{char.ToLowerInvariant(value[0])}{value[1..]}";

    private static string SplitPascalCase(string value) =>
        string.Concat(value.Select((character, index) =>
            index > 0 && char.IsUpper(character) && char.IsLower(value[index - 1])
                ? $" {character}"
                : character.ToString()));

    private static int GetStatOrder(string statKey) => statKey switch
    {
        "maxHealth" => 0,
        "currentHealth" => 1,
        "damage" => 2,
        "armor" => 3,
        "speed" => 4,
        "criticalChance" => 5,
        "criticalDamage" => 6,
        "dodgeChance" => 7,
        "debuffResistance" => 8,
        _ => 99
    };
}
