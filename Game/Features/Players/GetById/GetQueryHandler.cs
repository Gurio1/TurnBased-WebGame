using Game.Application.SharedKernel;
using Game.Core.Abilities;
using Game.Core.PlayerProfile;
using Game.Core.PlayerProfile.Aggregates;
using Game.Persistence.Mongo;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Game.Features.Players.GetById;

public sealed class GetQueryHandler : IRequestHandler<GetQuery, Result<GamePlayer>>
{
    private readonly IMongoCollectionProvider provider;
    
    public GetQueryHandler(IMongoCollectionProvider provider) =>
        this.provider = provider;
    
    public async Task<Result<GamePlayer>> Handle(GetQuery request, CancellationToken cancellationToken)
    {
        var lookupResult = await provider.GetCollection<GamePlayer>().AsQueryable()
            .Where(p => p.Id == request.PlayerId)
            .WithAbilities(provider.GetCollection<Ability>())
            .FirstOrDefaultAsync(cancellationToken);
        
        lookupResult.Local.Abilities = lookupResult.Results.ToList();
        
        return lookupResult.Local is null
            ? Result<GamePlayer>.NotFound($"Unable to retrieve player with id '{request.PlayerId}'")
            : Result<GamePlayer>.Success(lookupResult.Local);
    }
}
