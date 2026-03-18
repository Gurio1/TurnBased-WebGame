using Game.Contracts;
using Game.SharedKernel;

namespace Game.Features.Players.EquipItem;

public sealed class EquipCommand : IRequest<Result<PlayerViewModel>>
{
    public required string PlayerId { get; set; }
    public required string ItemId { get; set; }
}
