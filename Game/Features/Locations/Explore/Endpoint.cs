using FastEndpoints;
using Game.Application.SharedKernel;
using Game.Core.Models;

namespace Game.Features.Locations.Explore;

public sealed class Endpoint : Endpoint<ExploreCommand, Item>
{
    private readonly IDispatcher dispatcher;
    
    public Endpoint(IDispatcher dispatcher) => this.dispatcher = dispatcher;
    
    public override void Configure()
    {
        Get(EndpointSettings.DefaultName + "/explore/{LocationName}");
        Description(x => x.Accepts<ExploreCommand>());
    }
    
    public override async Task HandleAsync(ExploreCommand req, CancellationToken ct)
    {
        var item = await dispatcher.DispatchAsync(req, ct);
        
        await SendAsync(item, cancellation: ct);
    }
}
