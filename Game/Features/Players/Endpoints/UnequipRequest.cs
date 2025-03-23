using FastEndpoints;

namespace Game.Features.Players.Endpoints;

public class UnequipRequest
{
    [FromClaim]
    public string PlayerId { get; set; }

    public string EquipmentSlot { get; set; }
}