using FastEndpoints;
using Game.Core.Models;
using Game.Core.SharedKernel;

namespace Game.Features.Players.EquipEquipment;

public sealed class EquipCommand : IRequest<ResultWithoutValue>
{
    [FromClaim] public required string PlayerId { get; set; }
    
    public required string ItemId { get; set; }
}
