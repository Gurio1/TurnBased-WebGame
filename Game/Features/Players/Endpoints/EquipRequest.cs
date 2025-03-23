using FastEndpoints;

namespace Game.Features.Players.Endpoints;

public class EquipRequest
{
    [FromClaim]
    public string PlayerId { get; set; }
    public string EquipmentId { get; set; }
}