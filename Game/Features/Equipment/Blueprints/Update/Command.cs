using Game.Core.Common;
using Game.Core.Equipment;

namespace Game.Features.Equipment.Blueprints.Update;

public sealed record Command(EquipmentBlueprint Blueprint) : IRequest<ResultWithoutValue>;
