using Game.Core.Models;
using Game.Core.SharedKernel;
using Game.Persistence.Mongo;
using Game.Persistence.Queries;

namespace Game.Application.Features.Monsters.GetMonster;

//TODO: Index monster name
public sealed class GetQueryHandler : IRequestHandler<GetQuery, Result<Monster>>
{
    private readonly GetMonsterQuery query;
    
    public GetQueryHandler(GetMonsterQuery query) => this.query = query;
    
    public async Task<Result<Monster>> Handle(GetQuery request, CancellationToken cancellationToken) =>
        await query.GetByNameAsync(request.MonsterName, cancellationToken);
}
