using FastEndpoints;

namespace Game.Features.Players.Endpoints;

public class GetByIdRequest
{
    [FromClaim]
    public string PlayerId { get; set; }
}