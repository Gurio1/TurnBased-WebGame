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
    private readonly IPlayersMongoRepository _playersMongoRepository;

    public Create(UserManager<User> userManager,ITokenFactory tokenFactory,IPlayersMongoRepository playersMongoRepository)
    {
        _userManager = userManager;
        _tokenFactory = tokenFactory;
        _playersMongoRepository = playersMongoRepository;
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

        var character = new Player() { AbilityIds = ["0", "1","2"],Stats = new Stats()
        {
            MaxHealth = 250,
            CriticalDamage = 1.3f,
            CriticalChance = 0.1f,
            Damage = 20f,
            CurrentHealth = 250f
        }};
        
        await _playersMongoRepository.CreateAsync(character);

        newUser.PlayerId = character.Id;

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