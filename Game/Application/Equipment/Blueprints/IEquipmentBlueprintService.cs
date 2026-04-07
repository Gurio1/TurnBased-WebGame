using Game.Core.Equipment;
using Game.SharedKernel.Results;

namespace Game.Application.Equipment.Blueprints;

public interface IEquipmentBlueprintService
{
    Task<Result<IReadOnlyCollection<EquipmentBlueprint>>> GetAll(CancellationToken ct = default);
    Task<Result<EquipmentBlueprint>> GetById(string blueprintId, CancellationToken ct = default);
    Task<Result<IReadOnlyCollection<EquipmentDefinition>>> GetEquipmentCatalog(CancellationToken ct = default);
    Task<Result<IReadOnlyCollection<EquipmentStatDefinition>>> GetStatCatalog(CancellationToken ct = default);
    Task<Result<EquipmentBlueprint>> Create(SaveEquipmentBlueprintModel model, CancellationToken ct = default);
    Task<Result<EquipmentBlueprint>> Update(string blueprintId, SaveEquipmentBlueprintModel model, CancellationToken ct = default);
    Task<ResultWithoutValue> Delete(string blueprintId, CancellationToken ct = default);
}
