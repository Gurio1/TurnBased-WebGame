namespace Game.SharedKernel.Contracts.Requests;

public sealed record DeletePlayerRequest
{
    public string PlayerId { get; init; } = string.Empty;
}
