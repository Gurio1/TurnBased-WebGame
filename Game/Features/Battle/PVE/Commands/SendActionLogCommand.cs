using Game.Core.SharedKernel;

namespace Game.Features.Battle;

public record SendActionLogCommand(string BattleId, string Message) : IRequest<ResultWithoutValue>;
