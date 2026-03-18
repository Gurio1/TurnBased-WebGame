using Game.Contracts;
using Game.Features.Players.Create;
using Game.Features.Players.Delete;
using Game.Features.Players.EquipItem;
using Game.Features.Players.GetById;
using Game.Features.Players.Sell;
using Game.Features.Players.UnequipItem;
using Game.SharedKernel;
using Game.Utilities;
using Game.Utilities.Extensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Game.Features.Players;

[Route("players")]
public sealed class PlayersController(IDispatcher dispatcher, UrlBuilder urlBuilder) : Game.Controllers.ApiControllerBase
{
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetCurrentPlayer(CancellationToken cancellationToken)
    {
        string? playerId = GetPlayerId();
        if (string.IsNullOrWhiteSpace(playerId))
            return Unauthorized();

        var result = await dispatcher.DispatchAsync(new GetQuery { PlayerId = playerId }, cancellationToken);
        return result.IsSuccess
            ? Ok(result.Value.ToViewModel(urlBuilder))
            : StatusCode(result.Error.Code, result.Error.Description);
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<IActionResult> CreatePlayer(CancellationToken cancellationToken)
    {
        var result = await dispatcher.DispatchAsync(new CreateCommand(), cancellationToken);
        return HandleResult(result);
    }

    [HttpDelete("{playerId}")]
    public async Task<IActionResult> DeletePlayer([FromRoute] string playerId, CancellationToken cancellationToken)
    {
        var result = await dispatcher.DispatchAsync(new DeleteCommand(playerId), cancellationToken);
        return HandleResult(result, StatusCodes.Status200OK);
    }

    [HttpPost("equip/{itemId}", Name = EndpointNames.EquipItemEndpoint)]
    [Authorize]
    public async Task<IActionResult> EquipItem([FromRoute] string itemId, CancellationToken cancellationToken)
    {
        string? playerId = GetPlayerId();
        if (string.IsNullOrWhiteSpace(playerId))
            return Unauthorized();

        var result = await dispatcher.DispatchAsync(new EquipCommand
        {
            PlayerId = playerId,
            ItemId = itemId
        }, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("sell/{itemId}", Name = EndpointNames.SellItemEndpoint)]
    [Authorize]
    public async Task<IActionResult> SellItem([FromRoute] string itemId, CancellationToken cancellationToken)
    {
        string? playerId = GetPlayerId();
        if (string.IsNullOrWhiteSpace(playerId))
            return Unauthorized();

        var result = await dispatcher.DispatchAsync(new SellCommand
        {
            PlayerId = playerId,
            ItemId = itemId
        }, cancellationToken);

        return HandleResult(result);
    }

    [HttpPost("unequip/{equipmentSlot}", Name = EndpointNames.UnequipItemEndpoint)]
    [Authorize]
    public async Task<IActionResult> UnequipItem([FromRoute] string equipmentSlot, CancellationToken cancellationToken)
    {
        string? playerId = GetPlayerId();
        if (string.IsNullOrWhiteSpace(playerId))
            return Unauthorized();

        var result = await dispatcher.DispatchAsync(new UnequipCommand
        {
            PlayerId = playerId,
            EquipmentSlot = equipmentSlot
        }, cancellationToken);

        return HandleResult(result);
    }
}
