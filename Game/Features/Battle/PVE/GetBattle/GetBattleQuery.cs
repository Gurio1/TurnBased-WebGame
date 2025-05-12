using Game.Core.SharedKernel;
using Game.Features.Battle.Models;

namespace Game.Application.Features.Battle.PVE.GetBattle;

public record GetBattleQuery(string BattleId) : IRequest<Result<PveBattle>>;
