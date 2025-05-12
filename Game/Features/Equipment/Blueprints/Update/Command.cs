using Game.Core.Equipment;
using Game.Core.SharedKernel;

namespace Game.Features.Equipment.Blueprints.Update;

public sealed record Command(EquipmentBlueprint Blueprint) : IRequest<ResultWithoutValue>;
