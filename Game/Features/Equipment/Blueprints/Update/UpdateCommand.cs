using Game.Core.Equipment;
using Game.Core.SharedKernel;

namespace Game.Features.Equipment.Blueprints.Update;

public sealed record UpdateCommand(EquipmentBlueprint Blueprint) : IRequest<ResultWithoutValue>;
