namespace UserManagementAPI.Middleware;

public sealed class ErrorHandlingMiddleware(
    RequestDelegate next,
    ILogger<ErrorHandlingMiddleware> log)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        try
        {
            await next(ctx);
        }
        catch (Exception ex)
        {
            log.LogError(ex, "Unhandled exception");

            if (ctx.Response.HasStarted)
            {
                throw;
            }

            ctx.Response.Clear();
            ctx.Response.StatusCode = StatusCodes.Status500InternalServerError;
            ctx.Response.ContentType = "application/json";
            await ctx.Response.WriteAsJsonAsync(
                new { error = "Internal server error." },
                ctx.RequestAborted);
        }
    }
}
