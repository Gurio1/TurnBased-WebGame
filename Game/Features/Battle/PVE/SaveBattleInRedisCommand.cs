using Game.Core.SharedKernel;
using Game.Features.Battle.Models;

namespace Game.Features.Battle.PVE;

public record SaveBattleInRedisCommand(PveBattle Battle) : IRequest<ResultWithoutValue>;
