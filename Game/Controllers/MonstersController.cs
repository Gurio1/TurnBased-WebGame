using System.Reflection;
using Game.Application.Monsters;
using Game.Contracts;
using Game.Contracts.Requests;
using Game.Core.Models;
using Game.SharedKernel.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Game.Controllers;

[Route("monsters")]
public sealed class MonstersController(IMonsterService monsterService) : GameApiControllerBase
{
    private static readonly Dictionary<string, PropertyInfo> StatProperties = typeof(Stats)
        .GetProperties(BindingFlags.Public | BindingFlags.Instance)
        .ToDictionary(property => ToCamelCase(property.Name), StringComparer.OrdinalIgnoreCase);

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetMonsters(CancellationToken cancellationToken)
    {
        var result = await monsterService.GetAll(cancellationToken);
        if (result.IsFailure)
        {
            return StatusCode(result.Error.Code, result.Error.Description);
        }

        var response = result.Value
            .OrderBy(monster => monster.Name)
            .Select(monster => new MonsterSummaryViewModel
            {
                Name = monster.Name,
                MaxHealth = monster.Stats.MaxHealth,
                Damage = monster.Stats.Damage,
                AbilityCount = monster.AbilityIds.Count,
                DropCount = monster.Drops.Count
            })
            .ToList();

        return Ok(response);
    }

    [HttpGet("{monsterName}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetMonster([FromRoute] string monsterName, CancellationToken cancellationToken)
    {
        var monsterResult = await monsterService.GetByName(monsterName, cancellationToken);
        if (monsterResult.IsFailure)
        {
            return StatusCode(monsterResult.Error.Code, monsterResult.Error.Description);
        }

        var itemCatalogResult = await monsterService.GetItemCatalog(cancellationToken);
        if (itemCatalogResult.IsFailure)
        {
            return StatusCode(itemCatalogResult.Error.Code, itemCatalogResult.Error.Description);
        }

        var statCatalogResult = await monsterService.GetStatCatalog(cancellationToken);
        if (statCatalogResult.IsFailure)
        {
            return StatusCode(statCatalogResult.Error.Code, statCatalogResult.Error.Description);
        }

        return Ok(ToDetailViewModel(monsterResult.Value, itemCatalogResult.Value, statCatalogResult.Value));
    }

    [HttpGet("catalog/items")]
    [Authorize]
    public async Task<IActionResult> GetItemCatalog(CancellationToken cancellationToken)
    {
        var result = await monsterService.GetItemCatalog(cancellationToken);
        if (result.IsFailure)
        {
            return StatusCode(result.Error.Code, result.Error.Description);
        }

        return Ok(result.Value.Select(item => new MonsterItemCatalogItemViewModel
        {
            TypeName = item.TypeName,
            ItemId = item.ItemId,
            Name = item.Name,
            ImageUrl = item.ImageUrl,
            Category = item.Category,
            IsEquipment = item.IsEquipment
        }));
    }

    [HttpGet("catalog/abilities")]
    [Authorize]
    public async Task<IActionResult> GetAbilityCatalog(CancellationToken cancellationToken)
    {
        var result = await monsterService.GetAbilityCatalog(cancellationToken);
        if (result.IsFailure)
        {
            return StatusCode(result.Error.Code, result.Error.Description);
        }

        return Ok(result.Value.Select(item => new MonsterAbilityCatalogItemViewModel
        {
            Id = item.Id,
            TypeName = item.TypeName,
            Name = item.Name,
            ImageUrl = item.ImageUrl,
            Cooldown = item.Cooldown
        }));
    }

    [HttpGet("catalog/stats")]
    [Authorize]
    public async Task<IActionResult> GetStatCatalog(CancellationToken cancellationToken)
    {
        var result = await monsterService.GetStatCatalog(cancellationToken);
        if (result.IsFailure)
        {
            return StatusCode(result.Error.Code, result.Error.Description);
        }

        return Ok(result.Value.Select(item => new MonsterStatCatalogItemViewModel
        {
            Key = item.Key,
            Name = item.Name,
            ValueType = item.ValueType,
            DefaultValue = item.DefaultValue
        }));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateMonster([FromBody] CreateMonsterRequest request, CancellationToken cancellationToken)
    {
        var result = await monsterService.Create(ToSaveModel(request), cancellationToken);
        if (result.IsFailure)
        {
            return StatusCode(result.Error.Code, result.Error.Description);
        }

        var itemCatalogResult = await monsterService.GetItemCatalog(cancellationToken);
        if (itemCatalogResult.IsFailure)
        {
            return StatusCode(itemCatalogResult.Error.Code, itemCatalogResult.Error.Description);
        }

        var statCatalogResult = await monsterService.GetStatCatalog(cancellationToken);
        if (statCatalogResult.IsFailure)
        {
            return StatusCode(statCatalogResult.Error.Code, statCatalogResult.Error.Description);
        }

        return CreatedAtAction(
            nameof(GetMonster),
            new { monsterName = result.Value.Name },
            ToDetailViewModel(result.Value, itemCatalogResult.Value, statCatalogResult.Value));
    }

    [HttpPut("{monsterName}")]
    [Authorize]
    public async Task<IActionResult> UpdateMonster(
        [FromRoute] string monsterName,
        [FromBody] UpdateMonsterRequest request,
        CancellationToken cancellationToken)
    {
        var result = await monsterService.Update(monsterName, ToSaveModel(request), cancellationToken);
        return HandleResult(result, StatusCodes.Status200OK);
    }

    [HttpDelete("{monsterName}")]
    [Authorize]
    public async Task<IActionResult> DeleteMonster([FromRoute] string monsterName, CancellationToken cancellationToken)
    {
        var result = await monsterService.Delete(monsterName, cancellationToken);
        return HandleResult(result, StatusCodes.Status200OK);
    }

    private static SaveMonsterModel ToSaveModel(CreateMonsterRequest request) =>
        new()
        {
            Name = request.Name,
            OverallDropChance = request.OverallDropChance,
            Stats = request.Stats
                .Select(item => new MonsterStatValueModel(item.Key, item.Value))
                .ToList(),
            Drops = request.Drops
                .Select(item => new MonsterDropModel(item.ItemTypeName, item.ItemId, item.Quantity, item.Weight))
                .ToList(),
            AbilityIds = request.AbilityIds.ToList()
        };

    private static SaveMonsterModel ToSaveModel(UpdateMonsterRequest request) =>
        new()
        {
            Name = request.Name,
            OverallDropChance = request.OverallDropChance,
            Stats = request.Stats
                .Select(item => new MonsterStatValueModel(item.Key, item.Value))
                .ToList(),
            Drops = request.Drops
                .Select(item => new MonsterDropModel(item.ItemTypeName, item.ItemId, item.Quantity, item.Weight))
                .ToList(),
            AbilityIds = request.AbilityIds.ToList()
        };

    private static MonsterDetailViewModel ToDetailViewModel(
        Monster monster,
        IReadOnlyCollection<ItemCatalogEntry> itemCatalog,
        IReadOnlyCollection<MonsterStatCatalogEntry> statCatalog)
    {
        var itemByKey = itemCatalog.ToDictionary(
            item => ComposeCatalogKey(item.TypeName, item.ItemId),
            StringComparer.OrdinalIgnoreCase);

        return new MonsterDetailViewModel
        {
            Name = monster.Name,
            OverallDropChance = monster.OverallDropChance,
            AbilityIds = monster.AbilityIds.ToList(),
            Stats = statCatalog
                .Select(stat => new MonsterStatValueViewModel
                {
                    Key = stat.Key,
                    Name = stat.Name,
                    ValueType = stat.ValueType,
                    Value = GetStatValue(monster.Stats, stat.Key)
                })
                .ToList(),
            Drops = monster.Drops
                .Select(drop =>
                {
                    itemByKey.TryGetValue(ComposeCatalogKey(drop.ItemTypeName, drop.ItemId), out var definition);

                    return new MonsterDropEntryViewModel
                    {
                        ItemTypeName = drop.ItemTypeName,
                        ItemId = drop.ItemId,
                        ItemName = definition?.Name ?? drop.ItemId,
                        ItemImageUrl = definition?.ImageUrl ?? string.Empty,
                        Category = definition?.Category ?? "Unknown",
                        Quantity = drop.Quantity,
                        Weight = drop.Weight
                    };
                })
                .ToList()
        };
    }

    private static double GetStatValue(Stats stats, string key)
    {
        if (!StatProperties.TryGetValue(key, out var property))
        {
            return 0;
        }

        var value = property.GetValue(stats);
        return value switch
        {
            int intValue => intValue,
            float floatValue => floatValue,
            double doubleValue => doubleValue,
            _ => 0
        };
    }

    private static string ComposeCatalogKey(string typeName, string itemId) => $"{typeName}::{itemId}";

    private static string ToCamelCase(string value) =>
        string.IsNullOrEmpty(value)
            ? value
            : $"{char.ToLowerInvariant(value[0])}{value[1..]}";
}
