using Game.Core.Common;
using Game.Core.Models;
using Game.Features.Battle.Models;

namespace Game.Features.Battle.PVE.Events;

public record DefeatMonsterCommand(Monster Monster, CombatPlayer CombatPlayer) : IRequest<ResultWithoutValue>;
