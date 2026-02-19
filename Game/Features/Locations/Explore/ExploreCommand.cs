using FastEndpoints;
using Game.Application.SharedKernel;
using Game.Core.Models;

namespace Game.Features.Locations.Explore;

public sealed class ExploreCommand : IRequest<Item>
{
    [FromClaim] public required string PlayerId { get; set; }
    
    public required string LocationName { get; set; }
}
