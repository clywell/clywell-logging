using Microsoft.AspNetCore.Http;

namespace Clywell.Core.Logging.Middleware;

/// <summary>
/// Middleware that generates and tracks request IDs for HTTP requests.
/// Request IDs uniquely identify each HTTP request for troubleshooting.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="RequestIdMiddleware"/> class.
/// </remarks>
/// <param name="next">The next middleware in the pipeline.</param>
public sealed class RequestIdMiddleware(RequestDelegate next)
{
    private const string RequestIdHeaderName = "X-Request-ID";
    private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));

    /// <summary>
    /// Processes the HTTP request, ensuring a request ID is present.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        // Try to get request ID from request header
        var requestId = context.Request.Headers[RequestIdHeaderName].FirstOrDefault();

        // Generate a new one if not provided
        if (string.IsNullOrWhiteSpace(requestId))
        {
            requestId = Guid.NewGuid().ToString();
        }

        // Store in enricher for logging
        Enrichers.RequestIdEnricher.CurrentRequestId = requestId;

        // Add to response headers for client tracking
        context.Response.Headers[RequestIdHeaderName] = requestId;

        // Store in HttpContext for easy access
        context.Items["RequestId"] = requestId;

        await _next(context);
    }
}
