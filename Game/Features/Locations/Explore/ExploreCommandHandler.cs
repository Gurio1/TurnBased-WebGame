using Game.Application.SharedKernel;
using Game.Core.PlayerProfile.Aggregates;
using Game.Features.Locations.GetByName;
using Game.Persistence.Mongo;
using MongoDB.Driver;

namespace Game.Features.Locations.Explore;

public sealed class ExploreCommandHandler : IRequestHandler<ExploreCommand, Result<ExploreResponse>>
{
    private readonly IDispatcher dispatcher;
    private readonly IMongoCollection<GamePlayer> collection;

    public ExploreCommandHandler(IMongoCollectionProvider provider, IDispatcher dispatcher)
    {
        this.dispatcher = dispatcher;
        collection = provider.GetCollection<GamePlayer>();
    }

    public async Task<Result<ExploreResponse>> Handle(ExploreCommand request, CancellationToken cancellationToken)
    {
        var locationResult = await dispatcher.DispatchAsync(new GetQuery(request.LocationName), cancellationToken);

        if (locationResult.IsFailure)
            return locationResult.AsError<ExploreResponse>();

        var player = await collection.Find(p => p.Id == request.PlayerId).FirstOrDefaultAsync(cancellationToken: cancellationToken);

        if (player is null)
            return Result<ExploreResponse>.NotFound($"Player with id '{request.PlayerId}' was not found.");

        var location = locationResult.Value;
        var loot = location.Explore(player);

        var updateDef = Builders<GamePlayer>.Update.Set(p => p.Inventory, player.Inventory);
        await collection.UpdateOneAsync(p => p.Id == player.Id, updateDef, cancellationToken: cancellationToken);

        if (loot is null)
        {
            return Result<ExploreResponse>.Success(new ExploreResponse
            {
                LocationName = location.Name,
                Message = "You explored the area but found nothing this time.",
                Quantity = 0,
            });
        }

        return Result<ExploreResponse>.Success(new ExploreResponse
        {
            LocationName = location.Name,
            Message = $"You found {loot.Quantity}x {loot.Item.Name}.",
            ItemName = loot.Item.Name,
            ItemImageUrl = loot.Item.ImageUrl,
            Quantity = loot.Quantity,
        });
    }
}
