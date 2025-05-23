using Game.Application.SharedKernel;
using Game.Core.Equipment.Generation;

namespace Game.Features.Equipment.Generation;

public sealed record GetBlueprintQuery(string EquipmentId) : IRequest<Result<BlueprintAttributes>>;
