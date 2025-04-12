using Game.Core;
using Game.Core.Equipment;

namespace Game.Features.Equipment;

public interface IEquipmentGenerator
{
    Task<Result<EquipmentBase>> GenerateEquipment(string equipmentType);
}