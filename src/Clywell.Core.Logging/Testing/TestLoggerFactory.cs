using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;

namespace Clywell.Core.Logging.Testing;

/// <summary>
/// Factory for creating test loggers with in-memory sinks.
/// Simplifies logger setup for unit tests.
/// </summary>
public static class TestLoggerFactory
{
    /// <summary>
    /// Creates a test logger that writes to an in-memory sink.
    /// </summary>
    /// <param name="sink">The in-memory sink to capture log events.</param>
    /// <param name="minimumLevel">The minimum log level. Defaults to Verbose.</param>
    /// <returns>A logger instance for testing.</returns>
    public static Microsoft.Extensions.Logging.ILogger CreateTestLogger(
        InMemoryLogSink sink,
        LogEventLevel minimumLevel = LogEventLevel.Verbose)
    {
        ArgumentNullException.ThrowIfNull(sink);

        var logger = new LoggerConfiguration()
            .MinimumLevel.Is(minimumLevel)
            .WriteTo.Sink(sink)
            .CreateLogger();

        var factory = new LoggerFactory()
            .AddSerilog(logger);

        return factory.CreateLogger("TestLogger");
    }

    /// <summary>
    /// Creates a test logger with output and returns both the logger and the sink.
    /// </summary>
    /// <param name="minimumLevel">The minimum log level. Defaults to Verbose.</param>
    /// <returns>A tuple containing the logger and the in-memory sink.</returns>
    public static (Microsoft.Extensions.Logging.ILogger Logger, InMemoryLogSink Sink) CreateTestLoggerWithSink(
        LogEventLevel minimumLevel = LogEventLevel.Verbose)
    {
        var sink = new InMemoryLogSink();
        var logger = CreateTestLogger(sink, minimumLevel);
        return (logger, sink);
    }

    /// <summary>
    /// Creates a test logger with a specific category name.
    /// </summary>
    /// <typeparam name="T">The type to use for the logger category.</typeparam>
    /// <param name="sink">The in-memory sink to capture log events.</param>
    /// <param name="minimumLevel">The minimum log level. Defaults to Verbose.</param>
    /// <returns>A logger instance for testing.</returns>
    public static Microsoft.Extensions.Logging.ILogger<T> CreateTestLogger<T>(
        InMemoryLogSink sink,
        LogEventLevel minimumLevel = LogEventLevel.Verbose)
    {
        ArgumentNullException.ThrowIfNull(sink);

        var logger = new LoggerConfiguration()
            .MinimumLevel.Is(minimumLevel)
            .WriteTo.Sink(sink)
            .CreateLogger();

        var factory = new LoggerFactory()
            .AddSerilog(logger);

        return factory.CreateLogger<T>();
    }

    /// <summary>
    /// Creates a test logger with category and returns both the logger and the sink.
    /// </summary>
    /// <typeparam name="T">The type to use for the logger category.</typeparam>
    /// <param name="minimumLevel">The minimum log level. Defaults to Verbose.</param>
    /// <returns>A tuple containing the logger and the in-memory sink.</returns>
    public static (Microsoft.Extensions.Logging.ILogger<T> Logger, InMemoryLogSink Sink) CreateTestLoggerWithSink<T>(
        LogEventLevel minimumLevel = LogEventLevel.Verbose)
    {
        var sink = new InMemoryLogSink();
        var logger = CreateTestLogger<T>(sink, minimumLevel);
        return (logger, sink);
    }

    /// <summary>
    /// Creates a Serilog logger that writes to an in-memory sink.
    /// </summary>
    /// <param name="sink">The in-memory sink to capture log events.</param>
    /// <param name="minimumLevel">The minimum log level. Defaults to Verbose.</param>
    /// <returns>A Serilog logger for testing.</returns>
    public static Serilog.ILogger CreateSerilogTestLogger(
        InMemoryLogSink sink,
        LogEventLevel minimumLevel = LogEventLevel.Verbose)
    {
        ArgumentNullException.ThrowIfNull(sink);

        return new LoggerConfiguration()
            .MinimumLevel.Is(minimumLevel)
            .WriteTo.Sink(sink)
            .CreateLogger();
    }

    /// <summary>
    /// Creates a Serilog logger and returns both the logger and the sink.
    /// </summary>
    /// <param name="minimumLevel">The minimum log level. Defaults to Verbose.</param>
    /// <returns>A tuple containing the Serilog logger and the in-memory sink.</returns>
    public static (Serilog.ILogger Logger, InMemoryLogSink Sink) CreateSerilogTestLoggerWithSink(
        LogEventLevel minimumLevel = LogEventLevel.Verbose)
    {
        var sink = new InMemoryLogSink();
        var logger = CreateSerilogTestLogger(sink, minimumLevel);
        return (logger, sink);
    }
}
