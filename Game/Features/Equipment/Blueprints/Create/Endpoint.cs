using System.Globalization;
using FastEndpoints;
using Game.Core.Common;
using Endpoint = Game.Features.Equipment.Blueprints.GetByEquipmentId.Endpoint;

namespace Game.Features.Equipment.Blueprints.Create;

public sealed class CreateEquipmentBlueprint(IDispatcher dispatcher) : Endpoint<Command>
{
    public override void Configure() => Post(EndpointSettings.EndpointName);
    
    public override async Task HandleAsync(Command req, CancellationToken ct)
    {
        var result = await dispatcher.Dispatch(req, ct);
        
        if (result.IsFailure)
        {
            await SendAsync(result.Error.Description, Convert.ToInt32(result.Error.Code, CultureInfo.InvariantCulture),
                ct);
            return;
        }
        
        await SendCreatedAtAsync<Endpoint>(new { result.Value.EquipmentId }, result.Value, cancellation: ct);
    }
}
