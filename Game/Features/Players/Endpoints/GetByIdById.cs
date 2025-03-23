using FastEndpoints;
using Game.Features.Battle.Contracts;
using MongoDB.Bson.Serialization;

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
        var player =await _playersService.CreateQuery()
            .GetById(req.PlayerId)
            .WithAbilities()
            .ExecuteAsync<HeroBattleModel>();
        
        await SendOkAsync(player,ct);
    }
}