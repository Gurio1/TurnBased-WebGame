using System.Globalization;
using FastEndpoints;
using Game.Core.SharedKernel;
using Game.Features.Players.Contracts;

namespace Game.Features.Players.GetById;

public sealed class Endpoint : Endpoint<GetCommand>
{
    private readonly IDispatcher dispatcher;
    
    public Endpoint(IDispatcher dispatcher) => this.dispatcher = dispatcher;
    public override void Configure() => Get(EndpointSettings.EndpointName + "/{PlayerId}");
    
    public override async Task HandleAsync(GetCommand req, CancellationToken ct)
    {
        var result = await dispatcher.DispatchAsync(req, ct);
        
        if (result.IsFailure)
        {
            await SendAsync(result.Error.Description,
                Convert.ToInt32(result.Error.Code, CultureInfo.InvariantCulture),
                ct);
            return;
        }
        
        await SendOkAsync(result.Value.ToViewModel(),ct);
    }
}
