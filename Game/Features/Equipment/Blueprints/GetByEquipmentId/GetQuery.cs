using Game.Core.Equipment;
using Game.SharedKernel;

namespace Game.Features.Equipment.Blueprints.GetByEquipmentId;

public sealed record GetQuery(string EquipmentId) : IRequest<Result<EquipmentBlueprint>>;
