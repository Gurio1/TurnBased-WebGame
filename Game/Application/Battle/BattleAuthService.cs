using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace Game.Application.Battle;

public sealed class BattleAuthService : IBattleAuthService
{
    private readonly IHttpContextAccessor httpContextAccessor;
    
    public BattleAuthService(IHttpContextAccessor httpContextAccessor) => this.httpContextAccessor = httpContextAccessor;
    public string? GetCurrentPlayerId(ClaimsPrincipal? user) => user?.FindFirstValue(IBattleAuthService.PlayerIdClaim);
    
    public string? TryGetBattleId(ClaimsPrincipal? user) => user?.FindFirstValue(IBattleAuthService.BattleIdClaim);
    
    public void AppendBattleIdToClaims(ClaimsPrincipal? user, string battleId)
    {
        if (user!.HasClaim(c => c.Type == IBattleAuthService.BattleIdClaim))
            return;
        
        var identity = user.Identity as ClaimsIdentity;
        identity?.AddClaim(new Claim("BattleId", battleId));
        
        var principal = new ClaimsPrincipal(identity!);
        httpContextAccessor.HttpContext!.SignInAsync(principal);
    }
}
