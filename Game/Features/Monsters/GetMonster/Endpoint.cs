using System.Globalization;
using FastEndpoints;
using Game.Core.Common;

namespace Game.Features.Monsters.GetMonster;

public sealed class Endpoint : Endpoint<GetQuery>
{
    private readonly IDispatcher dispatcher;
    
    public Endpoint(IDispatcher dispatcher) => this.dispatcher = dispatcher;
    
    public override void Configure()
    {
        Get(EndpointSettings.EndpointName + "/{MonsterName}");
        AllowAnonymous();
        Description(x => x.Accepts<GetQuery>());
    }
    
    public override async Task HandleAsync(GetQuery req, CancellationToken ct)
    {
        var monsterResult = await dispatcher.DispatchAsync(req, ct);
        
        if (monsterResult.IsFailure)
        {
            await SendAsync(monsterResult.Error.Description,
                Convert.ToInt32(monsterResult.Error.Code, CultureInfo.InvariantCulture), ct);
            return;
        }
        
        await SendOkAsync(monsterResult.Value, ct);
    }
}
