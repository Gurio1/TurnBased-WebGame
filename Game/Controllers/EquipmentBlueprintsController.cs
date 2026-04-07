using Game.Contracts;
using Game.Core.Equipment;
using Game.Application.Equipment.Blueprints;
using Game.Contracts.Requests;
using Game.SharedKernel.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace Game.Controllers;

[Route("equipmentBlueprints")]
public sealed class EquipmentBlueprintsController(IEquipmentBlueprintService blueprintService) : GameApiControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetBlueprints(CancellationToken cancellationToken)
    {
        var result = await blueprintService.GetAll(cancellationToken);
        if (result.IsFailure)
            return StatusCode(result.Error.Code, result.Error.Description);

        var equipmentCatalog = await blueprintService.GetEquipmentCatalog(cancellationToken);
        if (equipmentCatalog.IsFailure)
            return StatusCode(equipmentCatalog.Error.Code, equipmentCatalog.Error.Description);

        var equipmentById = equipmentCatalog.Value.ToDictionary(item => item.EquipmentId, StringComparer.OrdinalIgnoreCase);

        var response = result.Value
            .OrderBy(blueprint => blueprint.EquipmentId)
            .Select(blueprint => ToSummaryViewModel(blueprint, equipmentById))
            .ToList();

        return Ok(response);
    }

    [HttpGet("catalog/equipment")]
    [Authorize]
    public async Task<IActionResult> GetEquipmentCatalog(CancellationToken cancellationToken)
    {
        var catalogResult = await blueprintService.GetEquipmentCatalog(cancellationToken);
        if (catalogResult.IsFailure)
            return StatusCode(catalogResult.Error.Code, catalogResult.Error.Description);

        var blueprintsResult = await blueprintService.GetAll(cancellationToken);
        if (blueprintsResult.IsFailure)
            return StatusCode(blueprintsResult.Error.Code, blueprintsResult.Error.Description);

        var blueprintAssignments = blueprintsResult.Value.ToDictionary(
            blueprint => blueprint.EquipmentId,
            blueprint => blueprint.Id,
            StringComparer.OrdinalIgnoreCase);

        var response = catalogResult.Value
            .Select(item => new EquipmentBlueprintEquipmentCatalogItemViewModel
            {
                TypeName = item.TypeName,
                EquipmentId = item.EquipmentId,
                Name = item.Name,
                Slot = item.Slot,
                ImageUrl = item.ImageUrl,
                AssignedBlueprintId = blueprintAssignments.GetValueOrDefault(item.EquipmentId)
            })
            .ToList();

        return Ok(response);
    }

    [HttpGet("catalog/stats")]
    [Authorize]
    public async Task<IActionResult> GetStatCatalog(CancellationToken cancellationToken)
    {
        var result = await blueprintService.GetStatCatalog(cancellationToken);
        if (result.IsFailure)
            return StatusCode(result.Error.Code, result.Error.Description);

        return Ok(result.Value.Select(item => new EquipmentBlueprintStatCatalogItemViewModel
        {
            Key = item.Key,
            Name = item.Name
        }));
    }

    [HttpGet("{blueprintId}")]
    [Authorize]
    public async Task<IActionResult> GetBlueprintById([FromRoute] string blueprintId, CancellationToken cancellationToken)
    {
        var result = await blueprintService.GetById(blueprintId, cancellationToken);
        if (result.IsFailure)
            return StatusCode(result.Error.Code, result.Error.Description);

        var equipmentCatalog = await blueprintService.GetEquipmentCatalog(cancellationToken);
        if (equipmentCatalog.IsFailure)
            return StatusCode(equipmentCatalog.Error.Code, equipmentCatalog.Error.Description);

        var statCatalog = await blueprintService.GetStatCatalog(cancellationToken);
        if (statCatalog.IsFailure)
            return StatusCode(statCatalog.Error.Code, statCatalog.Error.Description);

        var equipmentById = equipmentCatalog.Value.ToDictionary(item => item.EquipmentId, StringComparer.OrdinalIgnoreCase);
        var statByKey = statCatalog.Value.ToDictionary(item => item.Key, StringComparer.OrdinalIgnoreCase);

        return Ok(ToDetailViewModel(result.Value, equipmentById, statByKey));
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> CreateBlueprint([FromBody] CreateEquipmentBlueprintRequest request, CancellationToken cancellationToken)
    {
        var result = await blueprintService.Create(ToSaveModel(request), cancellationToken);
        if (result.IsFailure)
            return StatusCode(result.Error.Code, result.Error.Description);

        return CreatedAtAction(
            nameof(GetBlueprintById),
            new { blueprintId = result.Value.Id },
            new EquipmentBlueprintCreatedViewModel { Id = result.Value.Id });
    }

    [HttpPut("{blueprintId}")]
    [Authorize]
    public async Task<IActionResult> UpdateBlueprint([FromRoute] string blueprintId, [FromBody] UpdateEquipmentBlueprintRequest request, CancellationToken cancellationToken)
    {
        var result = await blueprintService.Update(blueprintId, ToSaveModel(request), cancellationToken);
        if (result.IsFailure)
            return StatusCode(result.Error.Code, result.Error.Description);

        return Ok();
    }

    [HttpDelete("{blueprintId}")]
    [Authorize]
    public async Task<IActionResult> DeleteBlueprint([FromRoute] string blueprintId, CancellationToken cancellationToken)
    {
        var result = await blueprintService.Delete(blueprintId, cancellationToken);
        return HandleResult(result, StatusCodes.Status200OK);
    }

    private static SaveEquipmentBlueprintModel ToSaveModel(CreateEquipmentBlueprintRequest request) =>
        new()
        {
            EquipmentId = request.EquipmentId,
            Stats = request.Stats.Select(stat => new BlueprintStatRange
            {
                StatKey = stat.StatKey,
                MinValue = stat.MinValue,
                MaxValue = stat.MaxValue
            }).ToList(),
            CountWeights = request.CountWeights
                .Select(weight => new KeyValuePair<int, double>(weight.Count, weight.Weight))
                .ToList()
        };

    private static SaveEquipmentBlueprintModel ToSaveModel(UpdateEquipmentBlueprintRequest request) =>
        new()
        {
            EquipmentId = request.EquipmentId,
            Stats = request.Stats.Select(stat => new BlueprintStatRange
            {
                StatKey = stat.StatKey,
                MinValue = stat.MinValue,
                MaxValue = stat.MaxValue
            }).ToList(),
            CountWeights = request.CountWeights
                .Select(weight => new KeyValuePair<int, double>(weight.Count, weight.Weight))
                .ToList()
        };

    private static EquipmentBlueprintSummaryViewModel ToSummaryViewModel(
        EquipmentBlueprint blueprint,
        Dictionary<string, EquipmentDefinition> equipmentById)
    {
        var equipment = equipmentById.GetValueOrDefault(blueprint.EquipmentId);

        return new EquipmentBlueprintSummaryViewModel
        {
            Id = blueprint.Id,
            EquipmentId = blueprint.EquipmentId,
            EquipmentName = equipment?.Name ?? blueprint.EquipmentId,
            EquipmentSlot = equipment?.Slot ?? "Unknown",
            EquipmentImageUrl = equipment?.ImageUrl ?? string.Empty,
            StatCount = blueprint.Stats.Count,
            MaxRollCount = blueprint.AttributeCountWeights.Keys
                .Select(key => int.TryParse(key, out var count) ? count : 0)
                .DefaultIfEmpty()
                .Max()
        };
    }

    private static EquipmentBlueprintDetailViewModel ToDetailViewModel(
        EquipmentBlueprint blueprint,
        Dictionary<string, EquipmentDefinition> equipmentById,
        Dictionary<string, EquipmentStatDefinition> statByKey)
    {
        var equipment = equipmentById.GetValueOrDefault(blueprint.EquipmentId);

        return new EquipmentBlueprintDetailViewModel
        {
            Id = blueprint.Id,
            EquipmentId = blueprint.EquipmentId,
            EquipmentName = equipment?.Name ?? blueprint.EquipmentId,
            EquipmentSlot = equipment?.Slot ?? "Unknown",
            EquipmentImageUrl = equipment?.ImageUrl ?? string.Empty,
            Stats = blueprint.Stats
                .OrderBy(stat => stat.StatKey)
                .Select(stat => new EquipmentBlueprintStatRangeViewModel
                {
                    StatKey = stat.StatKey,
                    StatName = statByKey.TryGetValue(stat.StatKey, out var definition)
                        ? definition.Name
                        : stat.StatKey,
                    MinValue = NormalizeStatValue(stat.StatKey, stat.MinValue),
                    MaxValue = NormalizeStatValue(stat.StatKey, stat.MaxValue)
                })
                .ToList(),
            CountWeights = blueprint.AttributeCountWeights
                .Select(entry => new EquipmentBlueprintCountWeightViewModel
                {
                    Count = int.Parse(entry.Key, CultureInfo.InvariantCulture),
                    Weight = entry.Value
                })
                .OrderBy(entry => entry.Count)
                .ToList()
        };
    }

    private static float NormalizeStatValue(string statKey, float value) =>
        statKey switch
        {
            nameof(CriticalChanceStat) => CriticalStatPercentages.NormalizeCriticalChance(value),
            nameof(CriticalDamageStat) => CriticalStatPercentages.NormalizeCriticalDamage(value),
            _ => value
        };
}

