using Serilog.Core;
using Serilog.Events;

namespace Clywell.Core.Logging.Enrichers;

/// <summary>
/// Enriches log events with a unique request ID.
/// Request IDs help track individual HTTP requests through the application.
/// </summary>
public sealed class RequestIdEnricher : ILogEventEnricher
{
    private const string RequestIdPropertyName = "RequestId";
    private static readonly AsyncLocal<string?> RequestId = new();

    /// <summary>
    /// Gets or sets the request ID for the current execution context.
    /// </summary>
    public static string? CurrentRequestId
    {
        get => RequestId.Value;
        set => RequestId.Value = value;
    }

    /// <summary>
    /// Enriches the log event with the current request ID.
    /// </summary>
    /// <param name="logEvent">The log event to enrich.</param>
    /// <param name="propertyFactory">Factory for creating log event properties.</param>
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        ArgumentNullException.ThrowIfNull(logEvent);

        ArgumentNullException.ThrowIfNull(propertyFactory);

        var requestId = CurrentRequestId;
        if (!string.IsNullOrEmpty(requestId))
        {
            var property = propertyFactory.CreateProperty(RequestIdPropertyName, requestId);
            logEvent.AddPropertyIfAbsent(property);
        }
    }
}
