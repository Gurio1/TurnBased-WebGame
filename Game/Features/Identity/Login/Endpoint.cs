using FastEndpoints;
using Game.Core.Models;
using Game.Features.Identity.Shared;
using Microsoft.AspNetCore.Identity;

namespace Game.Features.Identity.Login;

public sealed class Endpoint(UserManager<User> userManager, ITokenFactory tokenFactory) : Endpoint<Request>
{
    public override void Configure()
    {
        Post($"{EndpointSettings.EndpointName}/login");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(Request req, CancellationToken ct)
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
