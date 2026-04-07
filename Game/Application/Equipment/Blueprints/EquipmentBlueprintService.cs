using Game.Core.Equipment;
using Game.Core.Equipment.Generation;
using Game.SharedKernel.Results;
using Game.SharedKernel.Utilities;
using System.Globalization;

namespace Game.Application.Equipment.Blueprints;

public sealed class EquipmentBlueprintService(IEquipmentBlueprintRepository blueprintRepository) : IEquipmentBlueprintService
{
    public Task<Result<IReadOnlyCollection<EquipmentBlueprint>>> GetAll(CancellationToken ct = default) =>
        blueprintRepository.GetAll(ct);

    public Task<Result<EquipmentBlueprint>> GetById(string blueprintId, CancellationToken ct = default) =>
        blueprintRepository.GetById(blueprintId, ct);

    public Task<Result<IReadOnlyCollection<EquipmentDefinition>>> GetEquipmentCatalog(CancellationToken ct = default) =>
        Task.FromResult(Result<IReadOnlyCollection<EquipmentDefinition>>.Success(EquipmentFactory.GetDefinitions()));

    public Task<Result<IReadOnlyCollection<EquipmentStatDefinition>>> GetStatCatalog(CancellationToken ct = default) =>
        Task.FromResult(Result<IReadOnlyCollection<EquipmentStatDefinition>>.Success(EquipmentStatRegistry.GetDefinitions()));

    public Task<ResultWithoutValue> Delete(string blueprintId, CancellationToken ct = default) =>
        blueprintRepository.Delete(blueprintId, ct);

    public async Task<Result<EquipmentBlueprint>> Create(SaveEquipmentBlueprintModel model, CancellationToken ct = default)
    {
        var validationResult = await Validate(model, null, ct);
        if (validationResult.IsFailure)
            return Result<EquipmentBlueprint>.CustomError(validationResult.Error);

        var blueprint = BuildBlueprint(model);
        return await blueprintRepository.Create(blueprint, ct);
    }

    public async Task<Result<EquipmentBlueprint>> Update(string blueprintId, SaveEquipmentBlueprintModel model, CancellationToken ct = default)
    {
        var existingResult = await blueprintRepository.GetById(blueprintId, ct);
        if (existingResult.IsFailure)
            return existingResult;

        var validationResult = await Validate(model, blueprintId, ct);
        if (validationResult.IsFailure)
            return Result<EquipmentBlueprint>.CustomError(validationResult.Error);

        existingResult.Value.EquipmentId = model.EquipmentId.Trim();
        existingResult.Value.Stats = model.Stats
            .Select(NormalizeStatRange)
            .ToList();
        existingResult.Value.AttributeCountWeights = model.CountWeights
            .OrderBy(entry => entry.Key)
            .ToDictionary(entry => entry.Key.ToString(CultureInfo.InvariantCulture), entry => entry.Value);

        var updateResult = await blueprintRepository.Update(existingResult.Value, ct);
        return updateResult.IsSuccess
            ? Result<EquipmentBlueprint>.Success(existingResult.Value)
            : Result<EquipmentBlueprint>.CustomError(updateResult.Error);
    }

    private static EquipmentBlueprint BuildBlueprint(SaveEquipmentBlueprintModel model) =>
        new()
        {
            EquipmentId = model.EquipmentId.Trim(),
            Stats = model.Stats
                .Select(NormalizeStatRange)
                .ToList(),
            AttributeCountWeights = model.CountWeights
                .OrderBy(entry => entry.Key)
                .ToDictionary(entry => entry.Key.ToString(CultureInfo.InvariantCulture), entry => entry.Value)
        };

    private async Task<ResultWithoutValue> Validate(SaveEquipmentBlueprintModel model, string? excludedBlueprintId, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(model.EquipmentId))
            return ResultWithoutValue.Invalid("Equipment is required.");

        if (model.Stats.Count == 0)
            return ResultWithoutValue.Invalid("At least one stat is required.");

        if (model.CountWeights.Count == 0)
            return ResultWithoutValue.Invalid("At least one count weight is required.");

        var equipmentExists = EquipmentFactory.GetDefinitions()
            .Any(definition => string.Equals(definition.EquipmentId, model.EquipmentId.Trim(), StringComparison.OrdinalIgnoreCase));

        if (!equipmentExists)
            return ResultWithoutValue.Invalid($"Equipment '{model.EquipmentId}' is not supported.");

        if (await blueprintRepository.IsEquipmentAssigned(model.EquipmentId.Trim(), excludedBlueprintId, ct))
            return ResultWithoutValue.Invalid($"Equipment '{model.EquipmentId}' is already assigned to another blueprint.");

        var statKeys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var stat in model.Stats)
        {
            if (string.IsNullOrWhiteSpace(stat.StatKey))
                return ResultWithoutValue.Invalid("Stat key is required.");

            if (!statKeys.Add(stat.StatKey.Trim()))
                return ResultWithoutValue.Invalid("Each stat can only be selected once per blueprint.");

            if (!EquipmentStatRegistry.IsSupported(stat.StatKey.Trim()))
                return ResultWithoutValue.Invalid($"Stat '{stat.StatKey}' is not supported.");

            if (stat.MinValue > stat.MaxValue)
                return ResultWithoutValue.Invalid($"Stat '{stat.StatKey}' has an invalid range.");
        }

        double totalWeight = 0;
        var counts = new HashSet<int>();
        foreach (var countWeight in model.CountWeights)
        {
            if (countWeight.Key < 1)
                return ResultWithoutValue.Invalid("Count weights must start from 1.");

            if (countWeight.Key > model.Stats.Count)
                return ResultWithoutValue.Invalid("Count weights cannot exceed the number of selected stats.");

            if (!counts.Add(countWeight.Key))
                return ResultWithoutValue.Invalid("Each count weight can only be configured once.");

            if (countWeight.Value < 0)
                return ResultWithoutValue.Invalid("Count weights cannot be negative.");

            totalWeight += countWeight.Value;
        }

        if (totalWeight <= 0)
            return ResultWithoutValue.Invalid("At least one count weight must be greater than zero.");

        return ResultWithoutValue.Success();
    }

    private static BlueprintStatRange NormalizeStatRange(BlueprintStatRange stat)
    {
        var trimmedKey = stat.StatKey.Trim();

        return trimmedKey switch
        {
            nameof(CriticalChanceStat) => new BlueprintStatRange
            {
                StatKey = trimmedKey,
                MinValue = CriticalStatPercentages.NormalizeCriticalChance(stat.MinValue),
                MaxValue = CriticalStatPercentages.NormalizeCriticalChance(stat.MaxValue)
            },
            nameof(CriticalDamageStat) => new BlueprintStatRange
            {
                StatKey = trimmedKey,
                MinValue = CriticalStatPercentages.NormalizeCriticalDamage(stat.MinValue),
                MaxValue = CriticalStatPercentages.NormalizeCriticalDamage(stat.MaxValue)
            },
            _ => new BlueprintStatRange
            {
                StatKey = trimmedKey,
                MinValue = stat.MinValue,
                MaxValue = stat.MaxValue
            }
        };
    }
}
