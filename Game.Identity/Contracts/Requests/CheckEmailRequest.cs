using System.ComponentModel.DataAnnotations;

namespace Game.Identity.Contracts.Requests;

public sealed class CheckEmailRequest
{
    [Required]
    [EmailAddress]
    public required string Email { get; set; }
}
