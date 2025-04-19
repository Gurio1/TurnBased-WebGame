using FastEndpoints;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Game.Features.Identity.IsEmailUnique;

public sealed class IsEmailUnique(UserManager<User> userManager) : Endpoint<IsEmailUniqueRequest>
{
    
    public override void Configure()
    {
        Post("/users/check-email");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(IsEmailUniqueRequest req, CancellationToken ct)
    {
        bool isNotUnique = await userManager.Users.AnyAsync(u => u.Email == req.Email, cancellationToken: ct);
        
        await SendOkAsync(isNotUnique, ct);
    }
}
