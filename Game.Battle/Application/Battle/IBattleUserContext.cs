using System.Security.Claims;

namespace Game.Battle.Application.Battle;

public interface IBattleUserContext
{
    protected const string PlayerIdClaim = "PlayerId";
    protected const string BattleIdClaim = "BattleId";
    string? GetCurrentPlayerId(ClaimsPrincipal? user);
    string? TryGetBattleId(ClaimsPrincipal? user);
    void AppendBattleIdToClaims(ClaimsPrincipal? user, string battleId);
}
