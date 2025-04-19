using Game.Core.Common;

namespace Game.Features.Equipment;

public sealed record GetBlueprintQuery(string EquipmentId) : IRequest<Result<BlueprintAttributes>>;
