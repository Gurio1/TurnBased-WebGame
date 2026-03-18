using Game.SharedKernel;

namespace Game.Features.Locations.Explore;

public sealed class ExploreCommand : IRequest<Result<ExploreResponse>>
{
    public required string PlayerId { get; init; }
    public required string LocationName { get; init; }
}
