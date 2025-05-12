using Game.Core.SharedKernel;

namespace Game.Application.Features.Battle.PVE.EndBattle;

public record EndBattleCommand(string BattleId) : IRequest<ResultWithoutValue>;
