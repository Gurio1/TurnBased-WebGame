using Game.Application.SharedKernel;
using Game.Core.Models;
using Game.Core.PlayerProfile.Aggregates;
using Game.Features.Locations.GetByName;
using Game.Persistence.Mongo;
using MongoDB.Driver;

namespace Game.Features.Locations.Explore;

public sealed class ExploreCommandHandler : IRequestHandler<ExploreCommand, Item>
{
    private readonly IDispatcher dispatcher;
    private readonly IMongoCollection<GamePlayer>? collection;
    
    public ExploreCommandHandler(IMongoCollectionProvider provider, IDispatcher dispatcher)
    {
        this.dispatcher = dispatcher;
        collection = provider.GetCollection<GamePlayer>();
    }
    
    public async Task<Item> Handle(ExploreCommand request, CancellationToken cancellationToken)
    {
        var location = await dispatcher.DispatchAsync(new GetQuery("wvcdsc"), cancellationToken);
        
        var player = await collection.Find(p => p.Id == request.PlayerId).FirstOrDefaultAsync(cancellationToken: cancellationToken);
        
        var item =  location.Explore(player);
        
        var updateDef = Builders<GamePlayer>.Update
            .Set(p => p.Inventory, player.Inventory);
        
        await collection.UpdateOneAsync(p => p.Id == player.Id, updateDef, cancellationToken: cancellationToken);
        
        return item;
    }
}
