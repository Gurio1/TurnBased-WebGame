using System.Globalization;
using FastEndpoints;
using Game.Core.Common;

namespace Game.Features.Equipment.Blueprints.Update;

public sealed class Endpoint(IDispatcher dispatcher) : Endpoint<UpdateCommand>
{
    public override void Configure() => Put(EndpointSettings.EndpointName);
    
    public override async Task HandleAsync(UpdateCommand req, CancellationToken ct)
    {
        var result = await dispatcher.DispatchAsync(req, ct);
        
        if (result.IsFailure)
        {
            await SendAsync(result.Error.Description, Convert.ToInt32(result.Error.Code, CultureInfo.InvariantCulture),
                ct);
            return;
        }
        
        await SendNoContentAsync(ct);
    }
}
