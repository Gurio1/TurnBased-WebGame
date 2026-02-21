using FastEndpoints;
using Game.Application.SharedKernel;

namespace Game.Features.Locations.Explore;

public sealed class ExploreCommand : IRequest<Result<ExploreResponse>>
{
    [FromClaim] public required string PlayerId { get; set; }

    public required string LocationName { get; set; }
}
