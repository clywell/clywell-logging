using Microsoft.AspNetCore.Http;

namespace Clywell.Core.Logging.Middleware;

/// <summary>
/// Middleware that generates and tracks correlation IDs for HTTP requests.
/// Correlation IDs are used to trace related operations across distributed systems.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="CorrelationIdMiddleware"/> class.
/// </remarks>
/// <param name="next">The next middleware in the pipeline.</param>
public sealed class CorrelationIdMiddleware(RequestDelegate next)
{
    private const string CorrelationIdHeaderName = "X-Correlation-ID";
    private readonly RequestDelegate _next = next ?? throw new ArgumentNullException(nameof(next));

    /// <summary>
    /// Processes the HTTP request, ensuring a correlation ID is present.
    /// </summary>
    /// <param name="context">The HTTP context.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);

        // Try to get correlation ID from request header
        var correlationId = context.Request.Headers[CorrelationIdHeaderName].FirstOrDefault();

        // Generate a new one if not provided
        if (string.IsNullOrWhiteSpace(correlationId))
        {
            correlationId = Guid.NewGuid().ToString();
        }

        // Store in enricher for logging
        Enrichers.CorrelationIdEnricher.CurrentCorrelationId = correlationId;

        // Add to response headers for client tracking
        context.Response.Headers[CorrelationIdHeaderName] = correlationId;

        // Store in HttpContext for easy access
        context.Items["CorrelationId"] = correlationId;

        await _next(context);
    }
}
