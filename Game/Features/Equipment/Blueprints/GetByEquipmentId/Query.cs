using Game.Core.Equipment;
using Game.Core.SharedKernel;

namespace Game.Features.Equipment.Blueprints.GetByEquipmentId;

public sealed record Query(string EquipmentId) : IRequest<Result<EquipmentBlueprint>>;
