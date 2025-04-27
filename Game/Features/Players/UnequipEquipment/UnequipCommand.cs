using FastEndpoints;
using Game.Core.Common;

namespace Game.Features.Players.UnequipEquipment;

public class UnequipCommand() : IRequest<ResultWithoutValue>
{
    [FromClaim]public required string PlayerId { get; init; }
    public required string EquipmentSlot { get; init; }
}
