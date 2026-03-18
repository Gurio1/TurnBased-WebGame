using Game.Core.Battle.PVE;
using Game.SharedKernel;

namespace Game.Features.Battle.PVE.ExecutePlayerTurn;

public record ExecutePlayerTurnCommand(string AbilityId, PveBattle PveBattle) : IRequest<ResultWithoutValue>;
