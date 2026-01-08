using System.Security.Claims;
using AmazonBestSellers.Domain.Entities;
using AmazonBestSellers.Infrastructure.Data;

namespace AmazonBestSellers.API.Middleware;

public class AuditLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<AuditLoggingMiddleware> _logger;

    public AuditLoggingMiddleware(RequestDelegate next, ILogger<AuditLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context, ApplicationDbContext dbContext)
    {
        var endpoint = context.GetEndpoint();
        var method = context.Request.Method;
        var path = context.Request.Path;

        await _next(context);

        if (ShouldLog(method, path))
        {
            var userId = GetUserIdFromContext(context);

            var auditLog = new AuditLog
            {
                UserId = userId,
                Action = $"{method} {path}",
                EntityType = GetEntityTypeFromPath(path),
                EntityId = GetEntityIdFromPath(path),
                IpAddress = context.Connection.RemoteIpAddress?.ToString()
            };

            dbContext.AuditLogs.Add(auditLog);
            await dbContext.SaveChangesAsync();

            _logger.LogInformation("Audit: {Action} by User {UserId} from {IpAddress}",
                auditLog.Action, userId, auditLog.IpAddress);
        }
    }

    private static bool ShouldLog(string method, string path)
    {
        if (method == "GET") return false;

        var pathsToLog = new[] { "/api/auth", "/api/favorites" };
        return pathsToLog.Any(p => path.ToString().StartsWith(p, StringComparison.OrdinalIgnoreCase));
    }

    private static int? GetUserIdFromContext(HttpContext context)
    {
        var userIdClaim = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return int.TryParse(userIdClaim, out var userId) ? userId : null;
    }

    private static string GetEntityTypeFromPath(string path)
    {
        if (path.ToString().Contains("auth", StringComparison.OrdinalIgnoreCase))
            return "User";
        if (path.ToString().Contains("favorites", StringComparison.OrdinalIgnoreCase))
            return "FavoriteProduct";

        return "Unknown";
    }

    private static string GetEntityIdFromPath(string path)
    {
        var segments = path.ToString().Split('/');
        return int.TryParse(segments.LastOrDefault(), out var id) ? id.ToString() : "0";
    }
}
