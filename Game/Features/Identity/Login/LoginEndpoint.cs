using FastEndpoints;
using Game.Features.Identity.Shared;
using Microsoft.AspNetCore.Identity;

namespace Game.Features.Identity.Login;

public sealed class LoginEndpoint(UserManager<User> userManager, ITokenFactory tokenFactory) : Endpoint<LoginRequest>
{
    public override void Configure()
    {
        Post("/users/login");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        var user = await userManager.FindByEmailAsync(req.Email);
        
        if (user == null)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }
        
        bool loginSuccessful = await userManager.CheckPasswordAsync(user, req.Password);
        
        if (!loginSuccessful)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }
        
        string token = tokenFactory.CreateToken(user, Config);
        
        await SendAsync(new { token }, cancellation: ct);
    }
}
