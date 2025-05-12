using Game.Core.SharedKernel;
using Game.Features.Battle.Models;

namespace Game.Features.Battle.PVE.Events;

public record DefeatPlayerCommand(CombatPlayer CombatPlayer) : IRequest<ResultWithoutValue>;
