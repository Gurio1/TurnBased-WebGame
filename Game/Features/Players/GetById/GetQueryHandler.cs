using Game.Core.Abilities;
using Game.Core.Models;
using Game.Core.SharedKernel;
using Game.Persistence.Mongo;
using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace Game.Features.Players.GetById;

public sealed class GetQueryHandler : IRequestHandler<GetQuery, Result<Player>>
{
    private readonly IMongoCollectionProvider provider;
    public GetQueryHandler(IMongoCollectionProvider provider) =>
        this.provider = provider;
    
    public async Task<Result<Player>> Handle(GetQuery request, CancellationToken cancellationToken)
    {
        var lookupResult = await provider.GetCollection<Player>().AsQueryable()
            .Where(p => p.Id == request.PlayerId)
            .WithAbilities(provider.GetCollection<Ability>())
            .FirstOrDefaultAsync(cancellationToken);
        
        lookupResult.Local.Abilities = lookupResult.Results.ToList();
        
        return lookupResult.Local is null
            ? Result<Player>.NotFound($"Unable to retrieve player with id '{request.PlayerId}'")
            : Result<Player>.Success(lookupResult.Local);
    }
}
