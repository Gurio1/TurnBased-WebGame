using System.Diagnostics;

namespace Game.Utilities.Middlewares;

public class ExecutionTimeMiddleware
{
    private readonly RequestDelegate _next;
    
    public ExecutionTimeMiddleware(RequestDelegate next) => _next = next;
    
    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        
        await _next(context);
        
        stopwatch.Stop();
        
        var logger = context.RequestServices.GetRequiredService<ILogger<ExecutionTimeMiddleware>>();
        logger.LogInformation("Request to {RequestPath} took {StopwatchElapsedMilliseconds}ms", context.Request.Path,
            stopwatch.ElapsedMilliseconds);
    }
}
