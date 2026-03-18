using Game.Contracts;
using Game.SharedKernel;

namespace Game.Features.Players.Sell;

public sealed class SellCommand : IRequest<Result<PlayerViewModel>>
{
    public required string PlayerId { get; set; }
    public required string ItemId { get; set; }
}
