using FastEndpoints;
using Game.Core.Models;
using Game.Features.Players;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Game.Features.Identity.Endpoints;


//TODO : Implement refresh token in FastEndpoints(https://fast-endpoints.com/docs/security#jwt-refresh-tokens)

public class Create : Endpoint<CreateRequest>
{
    private readonly UserManager<User> _userManager;
    private readonly ITokenFactory _tokenFactory;
    private readonly PlayersService _playersService;

    public Create(UserManager<User> userManager,ITokenFactory tokenFactory,PlayersService playersService)
    {
        _userManager = userManager;
        _tokenFactory = tokenFactory;
        _playersService = playersService;
    }

    public override void Configure()
    {
        Post("/users");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(CreateRequest req, CancellationToken ct)
    {
        
        var isEmailNotUnique = await _userManager.Users.AnyAsync(u => u.Email == req.Email, cancellationToken: ct);

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
        
        await _playersService.CreateAsync(character);

        newUser.PlayerId = character.Id.ToString();

        var result = await _userManager.CreateAsync(newUser, req.Password);

        if (!result.Succeeded)
        {
            foreach (var er in result.Errors)
            {
                AddError(er.Description);
            }
            ThrowIfAnyErrors();
        }
        
        var token = _tokenFactory.CreateToken(newUser, Config);

        await SendOkAsync(new { token },ct);
    }
}