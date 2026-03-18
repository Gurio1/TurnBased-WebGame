using System.ComponentModel.DataAnnotations;

namespace Game.Identity.Contracts.Requests;

public record CreateUserRequest
{
    [EmailAddress]
    public required string Email { get; set; }

    [MinLength(8)]
    public required string Password { get; set; }

    [Compare(nameof(Password), ErrorMessage = "Password and confirmed password should be equal.")]
    public required string ConfirmedPassword { get; set; }
}
