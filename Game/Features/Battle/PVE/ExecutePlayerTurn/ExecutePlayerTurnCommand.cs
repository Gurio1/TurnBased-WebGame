using Game.Application.SharedKernel;
using Game.Core.Battle.PVE;

namespace Game.Features.Battle.PVE.ExecutePlayerTurn;

public record ExecutePlayerTurnCommand(string AbilityId, PveBattle PveBattle) : IRequest<ResultWithoutValue>;
