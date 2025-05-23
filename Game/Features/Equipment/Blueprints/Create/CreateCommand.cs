using Game.Application.SharedKernel;
using Game.Core.Equipment;

namespace Game.Features.Equipment.Blueprints.Create;

public sealed record CreateCommand(string EquipmentId) : IRequest<Result<EquipmentBlueprint>>;
//List<AttributeRange> AttributeRanges,
//Dictionary<string, double> AttributeCountWeights
