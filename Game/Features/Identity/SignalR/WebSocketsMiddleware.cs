namespace Game.Features.Identity.SignalR;

public class WebSocketsMiddleware
{
    private readonly RequestDelegate next;

    public WebSocketsMiddleware(RequestDelegate next) => this.next = next;
    
    public async Task Invoke(HttpContext httpContext)
    {
        var request = httpContext.Request;

        // web sockets cannot pass headers so we must take the access token from query param and
        // add it to the header before authentication middleware runs
        if (request.Path.StartsWithSegments("/hubs", StringComparison.OrdinalIgnoreCase) &&
            request.Query.TryGetValue("access_token", out var accessToken))
        {
            request.Headers.Append("Authorization", $"Bearer {accessToken}");
        }

        await next(httpContext);
    }
}
