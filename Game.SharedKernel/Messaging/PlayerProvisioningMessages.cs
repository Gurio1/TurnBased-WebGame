namespace Game.SharedKernel.Messaging;

public sealed class CreatePlayerResponse
{
    public string? PlayerId { get; set; }
    public string? ErrorMessage { get; set; }
}

public sealed class DeletePlayerResponse
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
