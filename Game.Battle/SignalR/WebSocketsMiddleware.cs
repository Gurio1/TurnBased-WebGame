namespace Game.Battle.SignalR;

public class WebSocketsMiddleware
{
    private readonly RequestDelegate next;

    public WebSocketsMiddleware(RequestDelegate next) => this.next = next;

    public async Task Invoke(HttpContext httpContext)
    {
        var request = httpContext.Request;

        if (request.Path.StartsWithSegments("/hubs", StringComparison.OrdinalIgnoreCase) &&
            request.Query.TryGetValue("access_token", out var accessToken))
            request.Headers.Append("Authorization", $"Bearer {accessToken}");

        await next(httpContext);
    }
}
