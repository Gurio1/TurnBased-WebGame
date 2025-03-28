using FastEndpoints;
using Game.Core.Models;
using Game.Features.Players.Contracts;
using MongoDB.Bson;

namespace Game.Features.Players.Endpoints;

public class Get : Endpoint<GetByIdRequest>
{
    private readonly PlayersService _playersService;

    public Get(PlayersService playersService)
    {
        _playersService = playersService;

    }
    public override void Configure()
    {
        Get("/players");
    }

    public override async Task HandleAsync(GetByIdRequest req, CancellationToken ct)
    {
        var player = await _playersService.GetByIdWithAbilities(req.PlayerId);
        
        await SendOkAsync(player.ToViewModel(),ct);
    }
}