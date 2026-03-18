using Game.Core.PlayerProfile;
using Game.Core.PlayerProfile.Aggregates;
using Game.SharedKernel;

namespace Game.Features.Players.GetById;

public sealed class GetQuery : IRequest<Result<GamePlayer>>
{
    public required string PlayerId { get; set; }
}
