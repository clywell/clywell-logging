using Serilog.Events;

namespace Clywell.Core.Logging.Testing;

/// <summary>
/// Assertion helpers for testing logging behavior with InMemoryLogSink.
/// </summary>
public static class LogAssertions
{
    /// <summary>
    /// Asserts that a log event was captured at the specified level.
    /// </summary>
    /// <param name="sink">The in-memory log sink.</param>
    /// <param name="level">The expected log level.</param>
    /// <exception cref="InvalidOperationException">Thrown when no events are found at the specified level.</exception>
    public static void ShouldHaveLogged(this InMemoryLogSink sink, LogEventLevel level)
    {
        ArgumentNullException.ThrowIfNull(sink);

        if (!sink.HasEventsAt(level))
        {
            throw new InvalidOperationException($"Expected at least one log event at level {level}, but none were found.");
        }
    }

    /// <summary>
    /// Asserts that a log event was captured containing the specified message.
    /// </summary>
    /// <param name="sink">The in-memory log sink.</param>
    /// <param name="level">The expected log level.</param>
    /// <param name="messageContains">Text that should appear in the log message.</param>
    /// <exception cref="InvalidOperationException">Thrown when no matching events are found.</exception>
    public static void ShouldHaveLogged(this InMemoryLogSink sink, LogEventLevel level, string messageContains)
    {
        ArgumentNullException.ThrowIfNull(sink);
        ArgumentNullException.ThrowIfNull(messageContains);

        var events = sink.GetEventsForLevel(level);
        var matchingEvents = events.Where(e =>
            e.MessageTemplate.Text.Contains(messageContains, StringComparison.OrdinalIgnoreCase) ||
            e.RenderMessage().Contains(messageContains, StringComparison.OrdinalIgnoreCase)).ToList();

        if (matchingEvents.Count == 0)
        {
            throw new InvalidOperationException(
                $"Expected at least one {level} log event containing '{messageContains}', but none were found. " +
                $"Found {events.Count} {level} events total.");
        }
    }

    /// <summary>
    /// Asserts that no log events were captured at the specified level.
    /// </summary>
    /// <param name="sink">The in-memory log sink.</param>
    /// <param name="level">The log level that should not have been logged.</param>
    /// <exception cref="InvalidOperationException">Thrown when events are found at the specified level.</exception>
    public static void ShouldNotHaveLogged(this InMemoryLogSink sink, LogEventLevel level)
    {
        ArgumentNullException.ThrowIfNull(sink);

        var count = sink.GetEventsForLevel(level).Count;
        if (count > 0)
        {
            throw new InvalidOperationException($"Expected no log events at level {level}, but found {count}.");
        }
    }

    /// <summary>
    /// Asserts that a log event contains a specific property.
    /// </summary>
    /// <param name="sink">The in-memory log sink.</param>
    /// <param name="propertyName">The property name to check for.</param>
    /// <exception cref="InvalidOperationException">Thrown when no events contain the property.</exception>
    public static void ShouldHaveProperty(this InMemoryLogSink sink, string propertyName)
    {
        ArgumentNullException.ThrowIfNull(sink);
        ArgumentNullException.ThrowIfNull(propertyName);

        var events = sink.GetEventsWithProperty(propertyName);
        if (events.Count == 0)
        {
            throw new InvalidOperationException($"Expected at least one log event with property '{propertyName}', but none were found.");
        }
    }

    /// <summary>
    /// Asserts that a log event contains a specific property with a specific value.
    /// </summary>
    /// <param name="sink">The in-memory log sink.</param>
    /// <param name="propertyName">The property name.</param>
    /// <param name="propertyValue">The expected property value.</param>
    /// <exception cref="InvalidOperationException">Thrown when no events match.</exception>
    public static void ShouldHavePropertyValue(this InMemoryLogSink sink, string propertyName, object propertyValue)
    {
        ArgumentNullException.ThrowIfNull(sink);
        ArgumentNullException.ThrowIfNull(propertyName);
        ArgumentNullException.ThrowIfNull(propertyValue);

        var events = sink.GetEventsWithPropertyValue(propertyName, propertyValue);
        if (events.Count == 0)
        {
            throw new InvalidOperationException(
                $"Expected at least one log event with property '{propertyName}' = '{propertyValue}', but none were found.");
        }
    }

    /// <summary>
    /// Asserts that a log event contains an exception of the specified type.
    /// </summary>
    /// <typeparam name="TException">The exception type to check for.</typeparam>
    /// <param name="sink">The in-memory log sink.</param>
    /// <exception cref="InvalidOperationException">Thrown when no events contain the exception type.</exception>
    public static void ShouldHaveException<TException>(this InMemoryLogSink sink) where TException : Exception
    {
        ArgumentNullException.ThrowIfNull(sink);

        if (!sink.HasException<TException>())
        {
            throw new InvalidOperationException($"Expected at least one log event with exception type {typeof(TException).Name}, but none were found.");
        }
    }

    /// <summary>
    /// Asserts that exactly the specified number of events were logged.
    /// </summary>
    /// <param name="sink">The in-memory log sink.</param>
    /// <param name="expectedCount">The expected number of events.</param>
    /// <exception cref="InvalidOperationException">Thrown when the count doesn't match.</exception>
    public static void ShouldHaveEventCount(this InMemoryLogSink sink, int expectedCount)
    {
        ArgumentNullException.ThrowIfNull(sink);

        var actualCount = sink.Count;
        if (actualCount != expectedCount)
        {
            throw new InvalidOperationException($"Expected {expectedCount} log events, but found {actualCount}.");
        }
    }

    /// <summary>
    /// Asserts that exactly the specified number of events were logged at the given level.
    /// </summary>
    /// <param name="sink">The in-memory log sink.</param>
    /// <param name="level">The log level to count.</param>
    /// <param name="expectedCount">The expected number of events.</param>
    /// <exception cref="InvalidOperationException">Thrown when the count doesn't match.</exception>
    public static void ShouldHaveEventCount(this InMemoryLogSink sink, LogEventLevel level, int expectedCount)
    {
        ArgumentNullException.ThrowIfNull(sink);

        var actualCount = sink.GetEventsForLevel(level).Count;
        if (actualCount != expectedCount)
        {
            throw new InvalidOperationException($"Expected {expectedCount} {level} events, but found {actualCount}.");
        }
    }
}
