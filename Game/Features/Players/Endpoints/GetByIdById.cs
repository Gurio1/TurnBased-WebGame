using System.Globalization;
using FastEndpoints;
using Game.Features.Players.Contracts;

namespace Game.Features.Players.Endpoints;

public sealed class GetByIdById : Endpoint<GetByIdRequest>
{
    private readonly IPlayersMongoRepository playersMongoRepository;

    public GetByIdById(IPlayersMongoRepository playersMongoRepository) => 
        this.playersMongoRepository = playersMongoRepository;
    
    public override void Configure() => 
        Get("/players");
    
    public override async Task HandleAsync(GetByIdRequest req, CancellationToken ct)
    {
        var playerResult = await playersMongoRepository.GetByIdWithAbilities(req.PlayerId);

        if (playerResult.IsFailure)
        {
            await SendAsync(playerResult.Error.Description, Convert.ToInt32(playerResult.Error.Code,CultureInfo.InvariantCulture), ct);
            return;
        }

        await SendOkAsync(playerResult.Value.ToViewModel(),ct);
    }
}
