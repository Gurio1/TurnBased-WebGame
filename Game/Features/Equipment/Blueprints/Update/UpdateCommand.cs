using Game.Core.Equipment;
using Game.SharedKernel;

namespace Game.Features.Equipment.Blueprints.Update;

public sealed record UpdateCommand(EquipmentBlueprint Blueprint) : IRequest<ResultWithoutValue>;
