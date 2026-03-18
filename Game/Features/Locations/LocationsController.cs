using Game.Features.Locations.Explore;
using Game.Features.Locations.GetByName;
using Game.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Game.Features.Locations;

[Route("locations")]
public sealed class LocationsController(IDispatcher dispatcher) : Game.Controllers.ApiControllerBase
{
    [HttpGet("{locationName}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLocation([FromRoute] string locationName, CancellationToken cancellationToken)
    {
        var result = await dispatcher.DispatchAsync(new GetQuery(locationName), cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("explore/{locationName}")]
    [Authorize]
    public async Task<IActionResult> ExploreLocation([FromRoute] string locationName, CancellationToken cancellationToken)
    {
        string? playerId = GetPlayerId();
        if (string.IsNullOrWhiteSpace(playerId))
            return Unauthorized();

        var result = await dispatcher.DispatchAsync(new ExploreCommand
        {
            PlayerId = playerId,
            LocationName = locationName
        }, cancellationToken);

        return HandleResult(result);
    }
}
