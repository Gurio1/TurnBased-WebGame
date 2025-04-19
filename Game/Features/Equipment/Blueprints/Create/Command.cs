using Game.Core.Common;
using Game.Core.Equipment;

namespace Game.Features.Equipment.Blueprints.Create;

public sealed record Command(
    string EquipmentId,
    List<AttributeRange> AttributeRanges,
    Dictionary<string, double> AttributeCountWeights) : IRequest<Result<EquipmentBlueprint>>;
