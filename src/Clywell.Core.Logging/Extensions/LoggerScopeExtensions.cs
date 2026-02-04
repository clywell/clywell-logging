using Microsoft.Extensions.Logging;

namespace Clywell.Core.Logging.Extensions;

/// <summary>
/// Extension methods for creating logging scopes with strongly-typed properties.
/// Properties added to a scope are automatically included in all log messages within that scope.
/// </summary>
public static class LoggerScopeExtensions
{
    /// <summary>
    /// Begins a logging scope with a single property that will be included in all logs within the scope.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="propertyName">The name of the property.</param>
    /// <param name="propertyValue">The value of the property.</param>
    /// <returns>A disposable scope.</returns>
    public static IDisposable BeginPropertyScope(this ILogger logger, string propertyName, object? propertyValue)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(propertyName);

        return logger.BeginScope(new Dictionary<string, object?> { [propertyName] = propertyValue })!;
    }

    /// <summary>
    /// Begins a logging scope with two properties that will be included in all logs within the scope.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="property1">The first property as a tuple of (name, value).</param>
    /// <param name="property2">The second property as a tuple of (name, value).</param>
    /// <returns>A disposable scope.</returns>
    public static IDisposable BeginPropertyScope(
        this ILogger logger,
        (string Name, object? Value) property1,
        (string Name, object? Value) property2)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(property1.Name);
        ArgumentNullException.ThrowIfNull(property2.Name);

        return logger.BeginScope(new Dictionary<string, object?>
        {
            [property1.Name] = property1.Value,
            [property2.Name] = property2.Value
        })!;
    }

    /// <summary>
    /// Begins a logging scope with three properties that will be included in all logs within the scope.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="property1">The first property as a tuple of (name, value).</param>
    /// <param name="property2">The second property as a tuple of (name, value).</param>
    /// <param name="property3">The third property as a tuple of (name, value).</param>
    /// <returns>A disposable scope.</returns>
    public static IDisposable BeginPropertyScope(
        this ILogger logger,
        (string Name, object? Value) property1,
        (string Name, object? Value) property2,
        (string Name, object? Value) property3)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(property1.Name);
        ArgumentNullException.ThrowIfNull(property2.Name);
        ArgumentNullException.ThrowIfNull(property3.Name);

        return logger.BeginScope(new Dictionary<string, object?>
        {
            [property1.Name] = property1.Value,
            [property2.Name] = property2.Value,
            [property3.Name] = property3.Value
        })!;
    }

    /// <summary>
    /// Begins a logging scope with multiple properties from a dictionary.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="properties">Dictionary of properties to include in the scope.</param>
    /// <returns>A disposable scope.</returns>
    public static IDisposable BeginPropertyScope(this ILogger logger, Dictionary<string, object?> properties)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(properties);

        return logger.BeginScope(properties)!;
    }

    /// <summary>
    /// Begins a logging scope with multiple properties from params tuples.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="properties">Variable number of properties as tuples of (name, value).</param>
    /// <returns>A disposable scope.</returns>
    public static IDisposable BeginPropertyScope(this ILogger logger, params (string Name, object? Value)[] properties)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(properties);

        var props = new Dictionary<string, object?>(properties.Length);
        foreach (var (name, value) in properties)
        {
            ArgumentNullException.ThrowIfNull(name);
            props[name] = value;
        }

        return logger.BeginScope(props)!;
    }

    /// <summary>
    /// Begins a logging scope with tenant context.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="tenantId">The tenant identifier.</param>
    /// <returns>A disposable scope.</returns>
    public static IDisposable BeginTenantScope(this ILogger logger, string tenantId)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(tenantId);

        return logger.BeginPropertyScope("TenantId", tenantId);
    }

    /// <summary>
    /// Begins a logging scope with user context.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A disposable scope.</returns>
    public static IDisposable BeginUserScope(this ILogger logger, string userId)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(userId);

        return logger.BeginPropertyScope("UserId", userId);
    }

    /// <summary>
    /// Begins a logging scope with both tenant and user context.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="tenantId">The tenant identifier.</param>
    /// <param name="userId">The user identifier.</param>
    /// <returns>A disposable scope.</returns>
    public static IDisposable BeginTenantUserScope(this ILogger logger, string tenantId, string userId)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(tenantId);
        ArgumentNullException.ThrowIfNull(userId);

        return logger.BeginPropertyScope(
            ("TenantId", tenantId),
            ("UserId", userId));
    }

    /// <summary>
    /// Begins a logging scope with operation context.
    /// </summary>
    /// <param name="logger">The logger instance.</param>
    /// <param name="operationName">The name of the operation.</param>
    /// <param name="operationId">Optional unique identifier for the operation.</param>
    /// <returns>A disposable scope.</returns>
    public static IDisposable BeginOperationScope(this ILogger logger, string operationName, string? operationId = null)
    {
        ArgumentNullException.ThrowIfNull(logger);
        ArgumentNullException.ThrowIfNull(operationName);

        var properties = new Dictionary<string, object?>
        {
            ["OperationName"] = operationName
        };

        if (!string.IsNullOrWhiteSpace(operationId))
        {
            properties["OperationId"] = operationId;
        }

        return logger.BeginScope(properties)!;
    }
}
