using Game.Application.SharedKernel;

namespace Game.Core.Equipment.Generation;

public interface IEquipmentGenerator
{
    Task<Result<EquipmentBase>> GenerateEquipment(string equipmentType);
}
