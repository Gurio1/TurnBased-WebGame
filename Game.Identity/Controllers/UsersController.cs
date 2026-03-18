using System.Text.Json;
using Game.Identity.Contracts.Requests;
using Game.Identity.Contracts.Responses;
using Game.Identity.Core;
using Game.Identity.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Game.Identity.Controllers;

[ApiController]
[Route("users")]
public sealed class UsersController(
    UserManager<User> userManager,
    ITokenFactory tokenFactory,
    IHttpClientFactory httpClientFactory,
    IConfiguration configuration) : ControllerBase
{
    private const string GameApiClientName = "GameApi";

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

        var createPlayerResult = await CreatePlayerAsync(cancellationToken);
        if (!createPlayerResult.Success)
            return StatusCode(createPlayerResult.StatusCode, createPlayerResult.ErrorMessage);

        var newUser = new User
        {
            UserName = request.Email,
            Email = request.Email,
            PlayerId = createPlayerResult.PlayerId!
        };

        var createUserResult = await userManager.CreateAsync(newUser, request.Password);
        if (!createUserResult.Succeeded)
        {
            AddIdentityErrors(createUserResult);

            var deletePlayerResult = await DeletePlayerAsync(createPlayerResult.PlayerId!, cancellationToken);
            if (!deletePlayerResult.Success)
                return StatusCode(deletePlayerResult.StatusCode, deletePlayerResult.ErrorMessage);

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

    private async Task<(bool Success, string? PlayerId, int StatusCode, string? ErrorMessage)> CreatePlayerAsync(CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient(GameApiClientName);
        using var response = await client.PostAsync("/players", content: null, cancellationToken);

        string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            return (
                false,
                null,
                (int)response.StatusCode,
                string.IsNullOrWhiteSpace(responseBody)
                    ? "Unable to create player in the game service."
                    : responseBody);
        }

        return (true, ParsePlayerId(responseBody), StatusCodes.Status200OK, null);
    }

    private async Task<(bool Success, int StatusCode, string? ErrorMessage)> DeletePlayerAsync(string playerId, CancellationToken cancellationToken)
    {
        var client = httpClientFactory.CreateClient(GameApiClientName);
        using var response = await client.DeleteAsync($"/players/{Uri.EscapeDataString(playerId)}", cancellationToken);

        if (response.IsSuccessStatusCode)
            return (true, StatusCodes.Status204NoContent, null);

        string responseBody = await response.Content.ReadAsStringAsync(cancellationToken);
        return (
            false,
            (int)response.StatusCode,
            string.IsNullOrWhiteSpace(responseBody)
                ? $"Player '{playerId}' was created, but the cleanup delete call failed."
                : responseBody);
    }

    private void AddIdentityErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);
    }

    private static string ParsePlayerId(string responseBody)
    {
        if (string.IsNullOrWhiteSpace(responseBody))
            throw new InvalidOperationException("Game service returned an empty player creation response.");

        try
        {
            string? playerId = JsonSerializer.Deserialize<string>(responseBody);
            if (!string.IsNullOrWhiteSpace(playerId))
                return playerId;
        }
        catch (JsonException)
        {
            // Fall back to raw text for non-JSON responses.
        }

        return responseBody.Trim().Trim('"');
    }
}
