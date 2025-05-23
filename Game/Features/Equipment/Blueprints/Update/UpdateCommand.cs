using Game.Application.SharedKernel;
using Game.Core.Equipment;

namespace Game.Features.Equipment.Blueprints.Update;

public sealed record UpdateCommand(EquipmentBlueprint Blueprint) : IRequest<ResultWithoutValue>;
