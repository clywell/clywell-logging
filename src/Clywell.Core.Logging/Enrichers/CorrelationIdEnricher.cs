using Serilog.Core;
using Serilog.Events;

namespace Clywell.Core.Logging.Enrichers;

/// <summary>
/// Enriches log events with a correlation ID from the current execution context.
/// Correlation IDs help trace related operations across distributed systems.
/// </summary>
public sealed class CorrelationIdEnricher : ILogEventEnricher
{
    private const string CorrelationIdPropertyName = "CorrelationId";
    private static readonly AsyncLocal<string?> CorrelationId = new();

    /// <summary>
    /// Gets or sets the correlation ID for the current execution context.
    /// </summary>
    public static string? CurrentCorrelationId
    {
        get => CorrelationId.Value;
        set => CorrelationId.Value = value;
    }

    /// <summary>
    /// Enriches the log event with the current correlation ID.
    /// </summary>
    /// <param name="logEvent">The log event to enrich.</param>
    /// <param name="propertyFactory">Factory for creating log event properties.</param>
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        ArgumentNullException.ThrowIfNull(logEvent);

        ArgumentNullException.ThrowIfNull(propertyFactory);

        var correlationId = CurrentCorrelationId ?? Guid.NewGuid().ToString();
        var property = propertyFactory.CreateProperty(CorrelationIdPropertyName, correlationId);
        logEvent.AddPropertyIfAbsent(property);
    }
}
