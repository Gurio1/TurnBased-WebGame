using Game.Core.Equipment;
using Game.SharedKernel;

namespace Game.Features.Equipment.Blueprints.Create;

public sealed record CreateCommand(string EquipmentId) : IRequest<Result<EquipmentBlueprint>>;
//List<AttributeRange> AttributeRanges,
//Dictionary<string, double> AttributeCountWeights
