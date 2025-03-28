using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Game.Features.Identity.Endpoints;

internal class IsEmailUnique(UserManager<User> userManager) : Endpoint<IsEmailUniqueRequest>
{
    
    public override void Configure()
    {
        Post("/users/check-email");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(IsEmailUniqueRequest req, CancellationToken ct)
    {
        var isNotUnique = await userManager.Users.AnyAsync(u => u.Email == req.Email, cancellationToken: ct);
        
        await SendOkAsync(isNotUnique, ct);
    }
}