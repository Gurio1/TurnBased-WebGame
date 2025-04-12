using Game.Core;
using Game.Core.Equipment;

namespace Game.Features.Equipment;

public interface IEquipmentTemplateMongoRepository
{
    Task<Result<EquipmentTemplate>> GetByEquipmentIdAsync(string equipmentId);
    Task SaveAsync(EquipmentTemplate template);
}