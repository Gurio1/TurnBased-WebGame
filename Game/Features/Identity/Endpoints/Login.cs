using FastEndpoints;
using Microsoft.AspNetCore.Identity;

namespace Game.Features.Identity.Endpoints;

internal class Login(UserManager<User> userManager,ITokenFactory tokenFactory) : Endpoint<LoginRequest>
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

        var loginSuccessful = await userManager.CheckPasswordAsync(user, req.Password);

        if (!loginSuccessful)
        {
            await SendUnauthorizedAsync(ct);
            return;
        }
        
        var token = tokenFactory.CreateToken(user, Config);

        await SendAsync(new {token = token}, cancellation: ct);
    }
}