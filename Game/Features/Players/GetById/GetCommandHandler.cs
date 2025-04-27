using Game.Core.Abilities;
using Game.Core.Common;
using Game.Core.Models;
using Game.Data.Mongo;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Game.Features.Players.GetById;

public sealed class GetCommandHandler : IRequestHandler<GetCommand,Result<Player>>
{
    private readonly IMongoCollection<Player> collection;
    private readonly IMongoDatabase mongoDatabase;
    private readonly IOptions<MongoSettings> settings;
    
    public GetCommandHandler(IMongoClient mongoClient,
        IOptions<MongoSettings> settings,IMongoCollectionProvider<Player> provider)
    {
        this.settings = settings;
        collection = provider.Collection;
        
        mongoDatabase = mongoClient.GetDatabase(
            settings.Value.DatabaseName);
    }
    public async Task<Result<Player>> Handle(GetCommand request, CancellationToken cancellationToken)
    {
        if (!settings.Value.CollectionNames.TryGetValue(nameof(Ability), out string? collName))
            return Result<Player>.Failure($"No mongo collection name configured for type '{nameof(Ability)}'");
        
        try
        {
            var lookupResult = await collection.AsQueryable()
                .Where(p => p.Id == request.PlayerId)
                .WithAbilities(mongoDatabase.GetCollection<Ability>(collName))
                .FirstOrDefaultAsync(cancellationToken: cancellationToken);
            
            lookupResult.Local.Abilities = lookupResult.Results.ToList();
            
            return lookupResult.Local is null
                ? Result<Player>.NotFound($"Unable to retrieve player with id '{request.PlayerId}'")
                : Result<Player>.Success(lookupResult.Local);
        }
        catch (Exception e)
        {
            return Result<Player>.Failure(e.Message);
        }
    }
}
