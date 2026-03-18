using Game.Core.Equipment.Generation;
using Game.SharedKernel;

namespace Game.Features.Equipment.Generation;

public sealed record GetBlueprintQuery(string EquipmentId) : IRequest<Result<BlueprintAttributes>>;
