using FastEndpoints;
using Game.Core.Models;
using Game.Features.Players.Contracts;
using MongoDB.Bson;

namespace Game.Features.Players.Endpoints;

public class Get : Endpoint<GetByIdRequest>
{
    private readonly IPlayersMongoRepository _playersMongoRepository;

    public Get(IPlayersMongoRepository playersMongoRepository)
    {
        _playersMongoRepository = playersMongoRepository;

    }
    public override void Configure()
    {
        Get("/players");
    }

    public override async Task HandleAsync(GetByIdRequest req, CancellationToken ct)
    {
        var playerResult = await _playersMongoRepository.GetByIdWithAbilities(req.PlayerId);

        if (playerResult.IsFailure)
        {
            await SendAsync(playerResult.Error.Description, int.Parse(playerResult.Error.Code), ct);
            return;
        }
        
        await SendOkAsync(playerResult.Value.ToViewModel(),ct);
    }
}