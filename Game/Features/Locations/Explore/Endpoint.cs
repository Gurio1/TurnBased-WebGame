using System.Globalization;
using FastEndpoints;
using Game.Application.SharedKernel;

namespace Game.Features.Locations.Explore;

public sealed class Endpoint : Endpoint<ExploreCommand>
{
    private readonly IDispatcher dispatcher;

    public Endpoint(IDispatcher dispatcher) => this.dispatcher = dispatcher;

    public override void Configure()
    {
        Post(EndpointSettings.DefaultName + "/explore/{LocationName}");
        Description(x => x.Accepts<ExploreCommand>());
    }

    public override async Task HandleAsync(ExploreCommand req, CancellationToken ct)
    {
        var result = await dispatcher.DispatchAsync(req, ct);

        if (result.IsFailure)
        {
            await SendAsync(result.Error.Description, Convert.ToInt32(result.Error.Code, CultureInfo.InvariantCulture), ct);
            return;
        }

        await SendOkAsync(result.Value, ct);
    }
}
