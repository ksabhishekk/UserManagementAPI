using System.Security.Cryptography;
using System.Text;

namespace UserManagementAPI.Middleware;

public sealed class TokenAuthMiddleware(RequestDelegate next, IConfiguration config)
{
    public async Task InvokeAsync(HttpContext ctx)
    {
        var header = ctx.Request.Headers.Authorization.ToString();
        var configuredToken = config["Auth:Token"];
        var token = header.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase)
            ? header["Bearer ".Length..]
            : null;

        if (token is null
            || configuredToken is null
            || !CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(token),
                Encoding.UTF8.GetBytes(configuredToken)))
        {
            ctx.Response.Headers.WWWAuthenticate = "Bearer";
            ctx.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await ctx.Response.WriteAsJsonAsync(
                new { error = "Unauthorized." },
                ctx.RequestAborted);
            return;
        }

        await next(ctx);
    }
}
