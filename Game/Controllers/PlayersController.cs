using Game.Contracts;
using Game.Application.Players;
using Game.SharedKernel;
using Game.Utilities;
using Game.Utilities.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Game.Controllers;

[Route("players")]
public sealed class PlayersController(IPlayerService playerService, UrlBuilder urlBuilder) : GameApiControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetCurrentPlayer(CancellationToken cancellationToken)
    {
        string? playerId = GetPlayerId();
        if (string.IsNullOrWhiteSpace(playerId))
            return Unauthorized();

        var result = await playerService.GetById(playerId, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value.ToViewModel(urlBuilder))
            : StatusCode(result.Error.Code, result.Error.Description);
    }

    [HttpPost("equip/{itemId}", Name = EndpointNames.EquipItemEndpoint)]
    [Authorize]
    public async Task<IActionResult> EquipItem([FromRoute] string itemId, CancellationToken cancellationToken)
    {
        string? playerId = GetPlayerId();
        if (string.IsNullOrWhiteSpace(playerId))
            return Unauthorized();

        var result = await playerService.EquipItem(playerId, itemId, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("sell/{itemId}", Name = EndpointNames.SellItemEndpoint)]
    [Authorize]
    public async Task<IActionResult> SellItem([FromRoute] string itemId, CancellationToken cancellationToken)
    {
        string? playerId = GetPlayerId();
        if (string.IsNullOrWhiteSpace(playerId))
            return Unauthorized();

        var result = await playerService.SellItem(playerId, itemId, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("unequip/{equipmentSlot}", Name = EndpointNames.UnequipItemEndpoint)]
    [Authorize]
    public async Task<IActionResult> UnequipItem([FromRoute] string equipmentSlot, CancellationToken cancellationToken)
    {
        string? playerId = GetPlayerId();
        if (string.IsNullOrWhiteSpace(playerId))
            return Unauthorized();

        var result = await playerService.UnequipItem(playerId, equipmentSlot, cancellationToken);

        return HandleResult(result);
    }
}
