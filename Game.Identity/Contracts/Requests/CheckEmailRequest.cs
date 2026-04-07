using System.ComponentModel.DataAnnotations;

namespace Game.Identity.Contracts.Requests;

public sealed record CheckEmailRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; init; }
}
