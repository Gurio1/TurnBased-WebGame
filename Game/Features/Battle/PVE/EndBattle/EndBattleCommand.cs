using Game.Core.SharedKernel;

namespace Game.Features.Battle.PVE.EndBattle;

public record EndBattleCommand(string BattleId) : IRequest<ResultWithoutValue>;
