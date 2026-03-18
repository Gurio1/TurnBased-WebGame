using System.ComponentModel.DataAnnotations;
using Game.Core.PlayerProfile.ValueObjects;

namespace Game.Features.Monsters;

public sealed class CreateMonsterRequest
{
    [Required]
    public required string Name { get; init; }

    [Required]
    public required Stats Stats { get; init; }

    [Required]
    public required Dictionary<string, float> DropsTable { get; init; }

    [Required]
    public required List<string> AbilityIds { get; init; }
}
