using Game.Core.SharedKernel;
using Game.Features.Battle.Models;

namespace Game.Features.Battle.PVE.Commands;

public record SendBattleDataCommand(PveBattle Battle) : IRequest<ResultWithoutValue>;
