using Game.Core.Common;
using Game.Features.Battle.Models;

namespace Game.Features.Battle.PVE.Events;

public record SendBattleDataCommand(PveBattle Battle) : IRequest<ResultWithoutValue>;
