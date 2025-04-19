using Game.Core.Common;

namespace Game.Features.Battle;

public record SendActionLogCommand(string BattleId, string Message) : IRequest<ResultWithoutValue>;
