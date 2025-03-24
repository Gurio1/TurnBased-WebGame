using FastEndpoints;
using Game.Core.Models;
using Game.Features.Players;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Game.Features.Identity.Endpoints;


//TODO : Implement refresh token in FastEndpoints(https://fast-endpoints.com/docs/security#jwt-refresh-tokens)

public class Create(UserManager<User> userManager,ITokenFactory tokenFactory,PlayersService playersService) : Endpoint<CreateRequest>
{
    public override void Configure()
    {
        Post("/users");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(CreateRequest req, CancellationToken ct)
    {
        
        var isEmailNotUnique = await userManager.Users.AnyAsync(u => u.Email == req.Email, cancellationToken: ct);

        if (isEmailNotUnique)
        {
            ThrowError(request => request.Email,"This email is already taken");
        }
        
        var newUser = new User()
        {
            UserName = req.Email,
            Email = req.Email,
        };

        var character = new Hero() { AbilityIds = ["0", "1"] };
        
        await playersService.CreateAsync(character);

        newUser.PlayerId = character.Id.ToString();

        var result = await userManager.CreateAsync(newUser, req.Password);

        if (!result.Succeeded)
        {
            foreach (var er in result.Errors)
            {
                AddError(er.Description);
            }
            ThrowIfAnyErrors();
        }

        
        var token = tokenFactory.CreateToken(newUser, Config);

        await SendOkAsync(new { token },ct);
    }
}