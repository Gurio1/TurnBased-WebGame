using System.Globalization;
using FastEndpoints;
using Game.Core.Models;
using Game.Core.SharedKernel;

namespace Game.Features.Monsters.Create;

//TODO : Write validation for all endpoints
public sealed class Endpoint : Endpoint<Request>
{
    private readonly IDispatcher dispatcher;
    
    public Endpoint(IDispatcher dispatcher) => this.dispatcher = dispatcher;
    
    public override void Configure() =>
        Post(EndpointSettings.EndpointName);
    
    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var monster = new Monster
        {
            Name = req.Name, AbilityIds = req.AbilityIds, Stats = req.Stats, DropsTable = req.DropsTable
        };
        
        var result = await dispatcher.DispatchAsync(new CreateCommand(monster), ct);
        
        if (result.IsFailure)
        {
            await SendAsync(result.Error.Description, Convert.ToInt32(result.Error.Code, CultureInfo.InvariantCulture),
                ct);
            return;
        }
        
        await SendCreatedAtAsync<Get.Endpoint>(new { monster.Name }, result.Value, cancellation: ct);
    }
}
