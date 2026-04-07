using Game.SharedKernel;
using Game.SharedKernel.Results;

namespace Game.Core.Equipment.Generation;

public interface IEquipmentGenerator
{
    Task<Result<EquipmentBase>> GenerateEquipment(string equipmentType);
}
