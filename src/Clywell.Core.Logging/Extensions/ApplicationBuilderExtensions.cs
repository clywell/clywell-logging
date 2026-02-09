using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Clywell.Core.Logging.Configuration;
using Clywell.Core.Logging.Middleware;

namespace Clywell.Core.Logging.Extensions;

/// <summary>
/// Extension methods for configuring logging in ASP.NET Core applications.
/// These extensions are for WebApplicationBuilder and IApplicationBuilder registration and middleware setup.
/// </summary>
public static class ApplicationBuilderExtensions
{
    /// <summary>
    /// Adds logging with default configuration to the application.
    /// </summary>
    /// <param name="builder">The web application builder.</param>
    /// <param name="configure">Optional configuration action.</param>
    /// <returns>The web application builder for chaining.</returns>
    public static WebApplicationBuilder AddLogging(
        this WebApplicationBuilder builder,
        Action<ClywellLoggerConfiguration>? configure = null)
    {
        var loggerConfig = ClywellLoggerConfiguration.Create(builder.Configuration)
            .WithClywellDefaults()
            .WithConsoleSink()
            .ReadFromConfiguration();

        configure?.Invoke(loggerConfig);

        // Set the global logger
        Log.Logger = loggerConfig.Build();

        // Configure Serilog as the logging provider
        builder.Logging.ClearProviders();
        builder.Host.UseSerilog();

        return builder;
    }

    /// <summary>
    /// Adds correlation ID and request ID middleware to the application pipeline.
    /// Should be called early in the pipeline to ensure IDs are available for all requests.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IApplicationBuilder UseRequestTracking(this IApplicationBuilder app)
    {
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<RequestIdMiddleware>();
        return app;
    }

    /// <summary>
    /// Adds Serilog request logging middleware with defaults.
    /// Logs HTTP requests with timing, status codes, and enriched properties.
    /// </summary>
    /// <param name="app">The application builder.</param>
    /// <returns>The application builder for chaining.</returns>
    public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder app)
    {
        app.UseSerilogRequestLogging(options =>
        {
            options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000} ms";
            options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
            {
                diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                diagnosticContext.Set("RemoteIpAddress", httpContext.Connection.RemoteIpAddress);
                diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());

                if (httpContext.Items.TryGetValue("CorrelationId", out var correlationId))
                {
                    diagnosticContext.Set("CorrelationId", correlationId);
                }

                if (httpContext.Items.TryGetValue("RequestId", out var requestId))
                {
                    diagnosticContext.Set("RequestId", requestId);
                }
            };
        });

        return app;
    }
}
