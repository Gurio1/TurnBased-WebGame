using FastEndpoints;
using Game.Core.Models;
using Game.Features.Identity.Shared;
using Game.Features.Players;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Game.Features.Identity.CreateUser;


//TODO : Implement refresh token in FastEndpoints(https://fast-endpoints.com/docs/security#jwt-refresh-tokens)

public sealed class Create : Endpoint<CreateRequest>
{
    private readonly UserManager<User> userManager;
    private readonly ITokenFactory tokenFactory;
    private readonly IPlayersMongoRepository playersMongoRepository;

    public Create(UserManager<User> userManager,ITokenFactory tokenFactory,IPlayersMongoRepository playersMongoRepository)
    {
        this.userManager = userManager;
        this.tokenFactory = tokenFactory;
        this.playersMongoRepository = playersMongoRepository;
    }

    public override void Configure()
    {
        Post("/users");
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(CreateRequest req, CancellationToken ct)
    {
        
        bool isEmailNotUnique = await userManager.Users.AnyAsync(u => u.Email == req.Email, cancellationToken: ct);

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
        
        await playersMongoRepository.CreateAsync(character);

        newUser.PlayerId = character.Id;

        var result = await userManager.CreateAsync(newUser, req.Password);

        if (!result.Succeeded)
        {
            foreach (var er in result.Errors)
            {
                AddError(er.Description);
            }
            ThrowIfAnyErrors();
        }
        
        string token = tokenFactory.CreateToken(newUser, Config);

        await SendOkAsync(new { token },ct);
    }
}
