﻿using Game.Application.SharedKernel;
using Game.Core.Models;
using Game.Persistence.Requests;

namespace Game.Features.Monsters.Get;

//TODO: Index monster name
public sealed class GetQueryHandler : IRequestHandler<GetQuery, Result<Monster>>
{
    private readonly GetMonsterQuery query;
    
    public GetQueryHandler(GetMonsterQuery query) => this.query = query;
    
    public async Task<Result<Monster>> Handle(GetQuery request, CancellationToken cancellationToken) =>
        await query.GetByNameAsync(request.MonsterName, cancellationToken);
}
