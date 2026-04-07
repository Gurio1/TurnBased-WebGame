using Game.Application.Locations;
using Game.Application.Locations.Explore;
using Game.SharedKernel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Game.Controllers;

[Route("locations")]
public sealed class LocationsController(ILocationService locationService) : GameApiControllerBase
{
    [HttpGet("{locationName}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetLocation([FromRoute] string locationName, CancellationToken cancellationToken)
    {
        var result = await locationService.GetByName(locationName, cancellationToken);
        return HandleResult(result);
    }

    [HttpPost("explore/{locationName}")]
    [Authorize]
    public async Task<IActionResult> ExploreLocation([FromRoute] string locationName, CancellationToken cancellationToken)
    {
        string? playerId = GetPlayerId();
        if (string.IsNullOrWhiteSpace(playerId))
            return Unauthorized();

        var result = await locationService.Explore(playerId, locationName, cancellationToken);

        return HandleResult(result);
    }
}

