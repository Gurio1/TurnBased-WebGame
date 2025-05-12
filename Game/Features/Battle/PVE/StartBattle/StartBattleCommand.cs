using Game.Core.SharedKernel;
using Game.Features.Battle.Models;

namespace Game.Features.Battle.PVE.StartBattle;

public record StartBattleCommand(string MonsterName, string PlayerId) : IRequest<Result<PveBattle>>;
