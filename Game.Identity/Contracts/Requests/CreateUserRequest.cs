using System.ComponentModel.DataAnnotations;

namespace Game.Identity.Contracts.Requests;

public sealed record CreateUserRequest
{
    [EmailAddress]
    public required string Email { get; init; }

    [MinLength(8)]
    public required string Password { get; init; }

    [Compare(nameof(Password), ErrorMessage = "Password and confirmed password should be equal.")]
    public required string ConfirmedPassword { get; init; }
}
