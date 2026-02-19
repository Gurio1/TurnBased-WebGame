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
    private readonly IPlayerRepository playerRepository;
    
    public GetQueryHandler(IPlayerRepository playerRepository) => this.playerRepository = playerRepository;
    
    public async Task<Result<GamePlayer>> Handle(GetQuery request, CancellationToken cancellationToken) => 
        await playerRepository.GetByIdWithAbilities(request.PlayerId, cancellationToken);
}
