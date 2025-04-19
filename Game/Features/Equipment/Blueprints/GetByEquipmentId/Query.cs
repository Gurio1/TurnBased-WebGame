using Game.Core.Common;
using Game.Core.Equipment;

namespace Game.Features.Equipment.Blueprints.GetByEquipmentId;

public sealed record Query(string EquipmentId) : IRequest<Result<EquipmentBlueprint>>;
