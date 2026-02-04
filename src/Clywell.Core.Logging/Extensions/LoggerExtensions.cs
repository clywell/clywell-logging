using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Clywell.Core.Logging.Extensions;

/// <summary>
/// Logging extensions for ILogger with built-in IsEnabled checks for optimal memory allocation.
/// These methods are drop-in replacements for standard ILogger methods with automatic performance optimization.
/// </summary>
public static class LoggerExtensions
{
    /// <summary>
    /// Logs a trace message if Trace level is enabled.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="message">The message template.</param>
    /// <param name="args">Optional message template arguments.</param>
    public static void Trace(this ILogger logger, string message, params object?[]? args)
    {
        ArgumentNullException.ThrowIfNull(logger);

        if (logger.IsEnabled(LogLevel.Trace))
        {
            logger.LogTrace(message, args ?? []);
        }
    }

    /// <summary>
    /// Logs a debug message if Debug level is enabled.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="message">The message template.</param>
    /// <param name="args">Optional message template arguments.</param>
    public static void Debug(this ILogger logger, string message, params object?[]? args)
    {
        ArgumentNullException.ThrowIfNull(logger);

        if (logger.IsEnabled(LogLevel.Debug))
        {
            logger.LogDebug(message, args ?? []);
        }
    }

    /// <summary>
    /// Logs an information message if Information level is enabled.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="message">The message template.</param>
    /// <param name="args">Optional message template arguments.</param>
    public static void Info(this ILogger logger, string message, params object?[]? args)
    {
        ArgumentNullException.ThrowIfNull(logger);

        if (logger.IsEnabled(LogLevel.Information))
        {
            logger.LogInformation(message, args ?? []);
        }
    }

    /// <summary>
    /// Logs a warning message if Warning level is enabled.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="message">The message template.</param>
    /// <param name="args">Optional message template arguments.</param>
    public static void Warning(this ILogger logger, string message, params object?[]? args)
    {
        ArgumentNullException.ThrowIfNull(logger);

        if (logger.IsEnabled(LogLevel.Warning))
        {
            logger.LogWarning(message, args ?? []);
        }
    }

    /// <summary>
    /// Logs an error message if Error level is enabled.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="message">The message template.</param>
    /// <param name="args">Optional message template arguments.</param>
    public static void Error(this ILogger logger, string message, params object?[]? args)
    {
        ArgumentNullException.ThrowIfNull(logger);

        if (logger.IsEnabled(LogLevel.Error))
        {
            logger.LogError(message, args ?? []);
        }
    }

    /// <summary>
    /// Logs an error message with exception if Error level is enabled.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The message template.</param>
    /// <param name="args">Optional message template arguments.</param>
    public static void Error(this ILogger logger, Exception exception, string message, params object?[]? args)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(exception);

        if (logger.IsEnabled(LogLevel.Error))
        {
            logger.LogError(exception, message, args ?? []);
        }
    }

    /// <summary>
    /// Logs a critical message if Critical level is enabled.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="message">The message template.</param>
    /// <param name="args">Optional message template arguments.</param>
    public static void Critical(this ILogger logger, string message, params object?[]? args)
    {
        ArgumentNullException.ThrowIfNull(logger);

        if (logger.IsEnabled(LogLevel.Critical))
        {
            logger.LogCritical(message, args ?? []);
        }
    }

    /// <summary>
    /// Logs a critical message with exception if Critical level is enabled.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="exception">The exception to log.</param>
    /// <param name="message">The message template.</param>
    /// <param name="args">Optional message template arguments.</param>
    public static void Critical(this ILogger logger, Exception exception, string message, params object?[]? args)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(exception);

        if (logger.IsEnabled(LogLevel.Critical))
        {
            logger.LogCritical(exception, message, args ?? []);
        }
    }

    /// <summary>
    /// Measures and logs the execution time of an operation.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="operationName">The name of the operation being measured.</param>
    /// <param name="operation">The operation to execute and measure.</param>
    /// <returns>The result of the operation.</returns>
    public static T LogExecutionTime<T>(this ILogger logger, string operationName, Func<T> operation)
    {
        ArgumentNullException.ThrowIfNull(logger);

        ArgumentNullException.ThrowIfNull(operation);

        var stopwatch = Stopwatch.StartNew();
        try
        {
            return operation();
        }
        finally
        {
            stopwatch.Stop();
            logger.LogInformation("{OperationName} completed in {ElapsedMilliseconds}ms", operationName, stopwatch.ElapsedMilliseconds);
        }
    }

    /// <summary>
    /// Measures and logs the execution time of an async operation.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="operationName">The name of the operation being measured.</param>
    /// <param name="operation">The async operation to execute and measure.</param>
    /// <returns>A task representing the async operation with its result.</returns>
    public static async Task<T> LogExecutionTimeAsync<T>(this ILogger logger, string operationName, Func<Task<T>> operation)
    {
        ArgumentNullException.ThrowIfNull(logger);

        ArgumentNullException.ThrowIfNull(operation);

        var stopwatch = Stopwatch.StartNew();
        try
        {
            return await operation();
        }
        finally
        {
            stopwatch.Stop();
            logger.LogInformation("{OperationName} completed in {ElapsedMilliseconds}ms", operationName, stopwatch.ElapsedMilliseconds);
        }
    }

    /// <summary>
    /// Creates a logging scope with automatic disposal and timing.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="scopeName">The name of the scope.</param>
    /// <param name="properties">Additional properties to include in the scope.</param>
    /// <returns>A disposable scope that logs start and end times.</returns>
    public static IDisposable BeginTimedScope(this ILogger logger, string scopeName, Dictionary<string, object>? properties = null)
    {
        ArgumentNullException.ThrowIfNull(logger);

        return new TimedScope(logger, scopeName, properties ?? new Dictionary<string, object>());
    }

    private sealed class TimedScope : IDisposable
    {
        private readonly ILogger _logger;
        private readonly string _scopeName;
        private readonly Stopwatch _stopwatch;

        public TimedScope(ILogger logger, string scopeName, Dictionary<string, object> properties)
        {
            _logger = logger;
            _scopeName = scopeName;
            _stopwatch = Stopwatch.StartNew();

            _logger.LogInformation("Starting {ScopeName} with properties {Properties}", scopeName, properties);
        }

        public void Dispose()
        {
            _stopwatch.Stop();
            _logger.LogInformation("Completed {ScopeName} in {ElapsedMilliseconds}ms", _scopeName, _stopwatch.ElapsedMilliseconds);
        }
    }
}
