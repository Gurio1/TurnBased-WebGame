using System.Globalization;
using FastEndpoints;
using Game.Core.SharedKernel;
using Game.Features.Identity.Shared;
using Microsoft.AspNetCore.Identity;

namespace Game.Features.Identity.CreateUser;

//TODO : Implement refresh token in FastEndpoints(https://fast-endpoints.com/docs/security#jwt-refresh-tokens)

public sealed class Endpoint : Endpoint<CreateRequest>
{
    private readonly IDispatcher dispatcher;
    private readonly ITokenFactory tokenFactory;
    private readonly UserManager<User> userManager;
    
    public Endpoint(UserManager<User> userManager, ITokenFactory tokenFactory, IDispatcher dispatcher)
    {
        this.userManager = userManager;
        this.dispatcher = dispatcher;
        this.tokenFactory = tokenFactory;
    }
    
    public override void Configure()
    {
        Post(EndpointSettings.EndpointName);
        AllowAnonymous();
    }
    
    public override async Task HandleAsync(CreateRequest req, CancellationToken ct)
    {
        var newUser = new User { UserName = req.Email, Email = req.Email };
        
        string playerId = await CreatePlayerAsync(ct);
        
        newUser.PlayerId = playerId;
        
        await CreateUserAsync(newUser, req.Password, ct);
        
        string token = tokenFactory.CreateToken(newUser, Config);
        
        await SendOkAsync(new { token }, ct);
    }
    
    private async Task<string> CreatePlayerAsync(CancellationToken ct)
    {
        var result = await dispatcher.DispatchAsync(new CreatePlayerCommand(), ct);
        
        if (result.IsFailure)
            ThrowError(r => r, result.Error.Description,
                Convert.ToInt32(result.Error.Code, CultureInfo.InvariantCulture));
        
        return result.Value;
    }
    
    private async Task CreateUserAsync(User user, string password, CancellationToken ct)
    {
        var result = await userManager.CreateAsync(user, password);
        
        if (!result.Succeeded)
        {
            foreach (var error in result.Errors) AddError(error.Description);
            
            ThrowIfAnyErrors();
        }
    }
}
