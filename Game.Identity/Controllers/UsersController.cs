using Game.Identity.Contracts.Requests;
using Game.Identity.Contracts.Responses;
using Game.Identity.Core;
using Game.Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Game.Identity.Controllers;

[ApiController]
[Route("users")]
public sealed class UsersController(
    UserManager<User> userManager,
    ITokenFactory tokenFactory,
    IConfiguration configuration) : ControllerBase
{
    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserRequest request, CancellationToken cancellationToken)
    {
        if (await userManager.Users.AnyAsync(u => u.Email == request.Email, cancellationToken))
        {
            ModelState.AddModelError(nameof(CreateUserRequest.Email), "This email is already taken.");
            return ValidationProblem(ModelState);
        }

        if (!string.Equals(request.Password, request.ConfirmedPassword, StringComparison.Ordinal))
        {
            ModelState.AddModelError(nameof(CreateUserRequest.ConfirmedPassword), "Password and confirmed password should be equal.");
            return ValidationProblem(ModelState);
        }

        var gamePlayerProvisioningClient = HttpContext.RequestServices.GetRequiredService<IGamePlayerProvisioningClient>();
        var createPlayerResult = await gamePlayerProvisioningClient.CreatePlayerAsync(cancellationToken);
        if (createPlayerResult.IsFailure)
            return StatusCode(StatusCodes.Status502BadGateway, createPlayerResult.Error.Description);

        var newUser = new User
        {
            UserName = request.Email,
            Email = request.Email,
            PlayerId = createPlayerResult.Value.PlayerId!
        };

        var createUserResult = await userManager.CreateAsync(newUser, request.Password);
        if (!createUserResult.Succeeded)
        {
            AddIdentityErrors(createUserResult);

            var deletePlayerResult = await gamePlayerProvisioningClient.DeletePlayerAsync(createPlayerResult.Value.PlayerId!, cancellationToken);
            if (deletePlayerResult.IsFailure)
                return StatusCode(StatusCodes.Status502BadGateway, deletePlayerResult.Error.Description);

            return ValidationProblem(ModelState);
        }

        string token = tokenFactory.CreateToken(newUser, configuration);
        return Ok(new IdentityTokenResponse(token));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request, CancellationToken cancellationToken)
    {
        var user = await userManager.FindByEmailAsync(request.Email);
        if (user is null)
            return Unauthorized();

        bool loginSuccessful = await userManager.CheckPasswordAsync(user, request.Password);
        if (!loginSuccessful)
            return Unauthorized();

        string token = tokenFactory.CreateToken(user, configuration);
        return Ok(new IdentityTokenResponse(token));
    }

    [HttpPost("check-email")]
    [AllowAnonymous]
    public async Task<IActionResult> CheckEmail([FromBody] CheckEmailRequest request, CancellationToken cancellationToken)
    {
        bool isNotUnique = await userManager.Users.AnyAsync(u => u.Email == request.Email, cancellationToken);
        return Ok(isNotUnique);
    }

    private void AddIdentityErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);
    }
}
