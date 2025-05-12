using Game.Core.Models;
using Game.Core.SharedKernel;
using Game.Features.Battle.Models;

namespace Game.Features.Battle.PVE.Events;

public record DefeatMonsterCommand(Monster Monster, CombatPlayer CombatPlayer) : IRequest<ResultWithoutValue>;
