namespace UserManagementAPI.Middleware;

public sealed class RequestLoggingMiddleware(
    RequestDelegate next,
    ILogger<RequestLoggingMiddleware> log)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        await next(ctx);
        log.LogInformation(
            "{Method} {Path} -> {Status}",
            ctx.Request.Method,
            ctx.Request.Path,
            ctx.Response.StatusCode);
    }
}
