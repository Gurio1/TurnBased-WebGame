using Game.Core;
using Game.Core.Common;
using Game.Core.Equipment;

namespace Game.Features.EquipmentBlueprints;

public interface IEquipmentTemplateMongoRepository
{
    Task<Result<EquipmentBlueprint>> GetByEquipmentIdAsync(string equipmentId);
    Task SaveAsync(EquipmentBlueprint equipmentBlueprint);
}
