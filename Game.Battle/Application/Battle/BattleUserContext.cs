using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Game.Battle.Application.Battle;

public sealed class BattleUserContext : IBattleUserContext
{
    private readonly IHttpContextAccessor httpContextAccessor;
    
    public BattleUserContext(IHttpContextAccessor httpContextAccessor) => this.httpContextAccessor = httpContextAccessor;
    public string? GetCurrentPlayerId(ClaimsPrincipal? user) => user?.FindFirstValue(IBattleUserContext.PlayerIdClaim);
    
    public string? TryGetBattleId(ClaimsPrincipal? user) => user?.FindFirstValue(IBattleUserContext.BattleIdClaim);
    
    public void AppendBattleIdToClaims(ClaimsPrincipal? user, string battleId)
    {
        if (user!.HasClaim(c => c.Type == IBattleUserContext.BattleIdClaim))
            return;
        
        var identity = user.Identity as ClaimsIdentity;
        identity?.AddClaim(new Claim("BattleId", battleId));
        
        var principal = new ClaimsPrincipal(identity!);
        httpContextAccessor.HttpContext!.SignInAsync(principal);
    }
}
