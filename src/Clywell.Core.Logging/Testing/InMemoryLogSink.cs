using Serilog.Core;
using Serilog.Events;

namespace Clywell.Core.Logging.Testing;

/// <summary>
/// In-memory log sink for unit testing. Captures all log events for assertion and verification.
/// </summary>
public sealed class InMemoryLogSink : ILogEventSink
{
    private readonly List<LogEvent> _logEvents = [];
    private readonly object _lock = new();

    /// <summary>
    /// Gets all logged events.
    /// </summary>
    public IReadOnlyList<LogEvent> LoggedEvents
    {
        get
        {
            lock (_lock)
            {
                return _logEvents.ToList();
            }
        }
    }

    /// <summary>
    /// Emits a log event to the in-memory collection.
    /// </summary>
    /// <param name="logEvent">The log event to capture.</param>
    public void Emit(LogEvent logEvent)
    {
        ArgumentNullException.ThrowIfNull(logEvent);

        lock (_lock)
        {
            _logEvents.Add(logEvent);
        }
    }

    /// <summary>
    /// Clears all captured log events.
    /// </summary>
    public void Clear()
    {
        lock (_lock)
        {
            _logEvents.Clear();
        }
    }

    /// <summary>
    /// Gets log events filtered by minimum level.
    /// </summary>
    /// <param name="minimumLevel">The minimum log level to include.</param>
    /// <returns>Filtered log events.</returns>
    public IReadOnlyList<LogEvent> GetEvents(LogEventLevel minimumLevel)
    {
        lock (_lock)
        {
            return _logEvents.Where(e => e.Level >= minimumLevel).ToList();
        }
    }

    /// <summary>
    /// Gets log events for a specific level.
    /// </summary>
    /// <param name="level">The log level to filter by.</param>
    /// <returns>Filtered log events.</returns>
    public IReadOnlyList<LogEvent> GetEventsForLevel(LogEventLevel level)
    {
        lock (_lock)
        {
            return _logEvents.Where(e => e.Level == level).ToList();
        }
    }

    /// <summary>
    /// Gets log events containing a specific message template.
    /// </summary>
    /// <param name="messageTemplate">The message template to search for.</param>
    /// <returns>Matching log events.</returns>
    public IReadOnlyList<LogEvent> GetEventsContaining(string messageTemplate)
    {
        ArgumentNullException.ThrowIfNull(messageTemplate);

        lock (_lock)
        {
            return _logEvents
                .Where(e => e.MessageTemplate.Text.Contains(messageTemplate, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }
    }

    /// <summary>
    /// Gets log events with a specific property.
    /// </summary>
    /// <param name="propertyName">The property name to search for.</param>
    /// <returns>Matching log events.</returns>
    public IReadOnlyList<LogEvent> GetEventsWithProperty(string propertyName)
    {
        ArgumentNullException.ThrowIfNull(propertyName);

        lock (_lock)
        {
            return _logEvents
                .Where(e => e.Properties.ContainsKey(propertyName))
                .ToList();
        }
    }

    /// <summary>
    /// Gets log events with a specific property value.
    /// </summary>
    /// <param name="propertyName">The property name.</param>
    /// <param name="propertyValue">The property value to match.</param>
    /// <returns>Matching log events.</returns>
    public IReadOnlyList<LogEvent> GetEventsWithPropertyValue(string propertyName, object propertyValue)
    {
        ArgumentNullException.ThrowIfNull(propertyName);
        ArgumentNullException.ThrowIfNull(propertyValue);

        lock (_lock)
        {
            return _logEvents
                .Where(e => e.Properties.TryGetValue(propertyName, out var value) &&
                           value.ToString().Contains(propertyValue.ToString() ?? string.Empty))
                .ToList();
        }
    }

    /// <summary>
    /// Gets the count of logged events.
    /// </summary>
    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _logEvents.Count;
            }
        }
    }

    /// <summary>
    /// Checks if any events were logged at the specified level.
    /// </summary>
    /// <param name="level">The log level to check.</param>
    /// <returns>True if events exist at the specified level.</returns>
    public bool HasEventsAt(LogEventLevel level)
    {
        lock (_lock)
        {
            return _logEvents.Any(e => e.Level == level);
        }
    }

    /// <summary>
    /// Checks if any events contain the specified exception type.
    /// </summary>
    /// <typeparam name="TException">The exception type to search for.</typeparam>
    /// <returns>True if events contain the exception type.</returns>
    public bool HasException<TException>() where TException : Exception
    {
        lock (_lock)
        {
            return _logEvents.Any(e => e.Exception is TException);
        }
    }

    /// <summary>
    /// Gets events that contain exceptions.
    /// </summary>
    /// <returns>Log events with exceptions.</returns>
    public IReadOnlyList<LogEvent> GetEventsWithExceptions()
    {
        lock (_lock)
        {
            return _logEvents.Where(e => e.Exception != null).ToList();
        }
    }

    /// <summary>
    /// Gets events for a specific exception type.
    /// </summary>
    /// <typeparam name="TException">The exception type.</typeparam>
    /// <returns>Log events with the specified exception type.</returns>
    public IReadOnlyList<LogEvent> GetEventsWithException<TException>() where TException : Exception
    {
        lock (_lock)
        {
            return _logEvents.Where(e => e.Exception is TException).ToList();
        }
    }
}
