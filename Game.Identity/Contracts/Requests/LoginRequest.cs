using System.ComponentModel.DataAnnotations;

namespace Game.Identity.Contracts.Requests;

public sealed record LoginRequest
{
    [EmailAddress]
    public required string Email { get; init; }
    
    public required string Password { get; init; }
}
