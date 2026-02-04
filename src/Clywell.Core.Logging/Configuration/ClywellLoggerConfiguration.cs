using Microsoft.Extensions.Configuration;
using Serilog;
using Serilog.Events;
using Serilog.Formatting.Json;
using Clywell.Core.Logging.Enrichers;
using Clywell.Core.Logging.Policies;

namespace Clywell.Core.Logging.Configuration;

/// <summary>
/// Fluent configuration builder for Serilog with Clywell best practices.
/// Provides methods to configure sinks, enrichers, and log levels.
/// </summary>
/// <remarks>
/// Initializes a new instance of the <see cref="ClywellLoggerConfiguration"/> class.
/// </remarks>
/// <param name="appConfig">Optional application configuration for reading settings.</param>
public sealed class ClywellLoggerConfiguration(IConfiguration? appConfig = null)
{
    private readonly LoggerConfiguration _config = new LoggerConfiguration();
    private readonly IConfiguration? _appConfig = appConfig;

    /// <summary>
    /// Creates a new logger configuration with default Clywell settings.
    /// </summary>
    /// <param name="appConfig">Optional application configuration.</param>
    /// <returns>A configured logger builder.</returns>
    public static ClywellLoggerConfiguration Create(IConfiguration? appConfig = null)
    {
        return new ClywellLoggerConfiguration(appConfig);
    }

    /// <summary>
    /// Sets the minimum log level.
    /// </summary>
    /// <param name="level">The minimum log level.</param>
    /// <returns>The configuration builder for chaining.</returns>
    public ClywellLoggerConfiguration WithMinimumLevel(LogEventLevel level)
    {
        _config.MinimumLevel.Is(level);
        return this;
    }

    /// <summary>
    /// Adds console sink with structured output.
    /// </summary>
    /// <param name="useJson">Whether to use JSON formatting (default: false).</param>
    /// <returns>The configuration builder for chaining.</returns>
    public ClywellLoggerConfiguration WithConsoleSink(bool useJson = false)
    {
        if (useJson)
        {
            _config.WriteTo.Console(new JsonFormatter());
        }
        else
        {
            _config.WriteTo.Console(
                outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");
        }

        return this;
    }

    /// <summary>
    /// Adds file sink for persistent logging.
    /// </summary>
    /// <param name="path">The file path (default: "logs/app-.txt").</param>
    /// <param name="rollingInterval">The rolling interval (default: Daily).</param>
    /// <returns>The configuration builder for chaining.</returns>
    public ClywellLoggerConfiguration WithFileSink(
        string path = "logs/app-.txt",
        RollingInterval rollingInterval = RollingInterval.Day)
    {
        _config.WriteTo.File(
            path,
            rollingInterval: rollingInterval,
            retainedFileCountLimit: 31,
            outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}");

        return this;
    }

    /// <summary>
    /// Adds Seq sink for centralized logging.
    /// </summary>
    /// <param name="serverUrl">The Seq server URL (default: from configuration or http://localhost:5341).</param>
    /// <returns>The configuration builder for chaining.</returns>
    public ClywellLoggerConfiguration WithSeqSink(string? serverUrl = null)
    {
        serverUrl ??= _appConfig?["Logging:Seq:ServerUrl"] ?? "http://localhost:5341";
        _config.WriteTo.Seq(serverUrl);
        return this;
    }

    /// <summary>
    /// Adds Application Insights sink for Azure monitoring.
    /// </summary>
    /// <param name="connectionString">The Application Insights connection string.</param>
    /// <returns>The configuration builder for chaining.</returns>
    public ClywellLoggerConfiguration WithApplicationInsightsSink(string? connectionString = null)
    {
        connectionString ??= _appConfig?["ApplicationInsights:ConnectionString"];

        if (!string.IsNullOrWhiteSpace(connectionString))
        {
            _config.WriteTo.ApplicationInsights(connectionString, TelemetryConverter.Traces);
        }

        return this;
    }

    /// <summary>
    /// Adds correlation ID enricher for distributed tracing.
    /// </summary>
    /// <returns>The configuration builder for chaining.</returns>
    public ClywellLoggerConfiguration WithCorrelationId()
    {
        _config.Enrich.With<CorrelationIdEnricher>();
        return this;
    }

    /// <summary>
    /// Adds request ID enricher for HTTP request tracking.
    /// </summary>
    /// <returns>The configuration builder for chaining.</returns>
    public ClywellLoggerConfiguration WithRequestId()
    {
        _config.Enrich.With<RequestIdEnricher>();
        return this;
    }

    /// <summary>
    /// Adds environment enrichers (machine name, environment name).
    /// </summary>
    /// <returns>The configuration builder for chaining.</returns>
    public ClywellLoggerConfiguration WithEnvironmentEnrichers()
    {
        _config.Enrich.WithMachineName();
        _config.Enrich.FromLogContext();
        return this;
    }

    /// <summary>
    /// Adds sensitive data redaction for security.
    /// </summary>
    /// <returns>The configuration builder for chaining.</returns>
    public ClywellLoggerConfiguration WithSensitiveDataRedaction()
    {
        _config.Destructure.With<SensitiveDataRedactionPolicy>();
        return this;
    }

    /// <summary>
    /// Configures all default Clywell enrichers and settings.
    /// Includes correlation ID, request ID, environment info, and sensitive data redaction.
    /// </summary>
    /// <returns>The configuration builder for chaining.</returns>
    public ClywellLoggerConfiguration WithClywellDefaults()
    {
        return WithCorrelationId()
            .WithRequestId()
            .WithEnvironmentEnrichers()
            .WithSensitiveDataRedaction();
    }

    /// <summary>
    /// Overrides log level for a specific namespace.
    /// </summary>
    /// <param name="sourceContext">The namespace or source context.</param>
    /// <param name="level">The minimum log level for this context.</param>
    /// <returns>The configuration builder for chaining.</returns>
    public ClywellLoggerConfiguration OverrideMinimumLevel(string sourceContext, LogEventLevel level)
    {
        _config.MinimumLevel.Override(sourceContext, level);
        return this;
    }

    /// <summary>
    /// Applies configuration from appsettings.json.
    /// </summary>
    /// <returns>The configuration builder for chaining.</returns>
    public ClywellLoggerConfiguration ReadFromConfiguration()
    {
        if (_appConfig != null)
        {
            _config.ReadFrom.Configuration(_appConfig);
        }

        return this;
    }

    /// <summary>
    /// Builds and returns the configured Serilog logger.
    /// </summary>
    /// <returns>The configured Serilog logger.</returns>
    public ILogger Build()
    {
        return _config.CreateLogger();
    }

    /// <summary>
    /// Builds and sets the global logger instance (Log.Logger).
    /// </summary>
    public void BuildAndSetGlobalLogger()
    {
        Log.Logger = Build();
    }
}
