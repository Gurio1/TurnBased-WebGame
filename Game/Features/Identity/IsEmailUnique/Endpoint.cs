using FastEndpoints;
using Game.Core.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Game.Features.Identity.IsEmailUnique;

public sealed class Endpoint(UserManager<User> userManager) : Endpoint<Request>
{
    public override void Configure()
    {
        Post($"{EndpointSettings.EndpointName}/check-email");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        bool isNotUnique = await userManager.Users.AnyAsync(u => u.Email == req.Email, ct);
        
        await SendOkAsync(isNotUnique, ct);
    }
}
