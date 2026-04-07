using Game.SharedKernel;
using Game.SharedKernel.Results;

namespace Game.Core.Equipment;

public interface IEquipmentBlueprintRepository
{
    Task<Result<IReadOnlyCollection<EquipmentBlueprint>>> GetAll(CancellationToken ct = default);
    Task<Result<EquipmentBlueprint>> GetById(string blueprintId, CancellationToken ct = default);
    Task<Result<EquipmentBlueprint>> GetByEquipmentId(string equipmentId, CancellationToken ct = default);
    Task<Result<EquipmentBlueprint>> Create(EquipmentBlueprint blueprint, CancellationToken ct = default);
    Task<ResultWithoutValue> Update(EquipmentBlueprint blueprint, CancellationToken ct = default);
    Task<ResultWithoutValue> Delete(string blueprintId, CancellationToken ct = default);
    Task<bool> IsEquipmentAssigned(string equipmentId, string? excludedBlueprintId = null, CancellationToken ct = default);
}
