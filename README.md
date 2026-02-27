Note: The tool simplified the command to ` cat > /Users/sodiqyekeen/Documents/dev/clywell-core/clywell-logging/README.md << 'ENDOFFILE'
# Clywell.Core.Logging

[![NuGet](https://img.shields.io/nuget/v/Clywell.Core.Logging.svg)](https://www.nuget.org/packages/Clywell.Core.Logging/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Structured logging infrastructure with Serilog for the Clywell platform. Provides log enrichers, configuration helpers, and performance-optimized logging utilities with **zero business logic** - can be used in any .NET application.

---

## Features

✅ **Serilog Integration** - Fluent configuration for Serilog with best practices  
✅ **Correlation & Request Tracking** - Automatic correlation IDs and request IDs for distributed tracing  
✅ **Performance Optimized** - `IsEnabled()` checks built into shorthand extension methods  
✅ **Sensitive Data Redaction** - Automatic redaction of passwords, credit cards, API keys  
✅ **Multiple Sinks** - Console, File, Seq, Application Insights  
✅ **Environment Enrichers** - Machine name and log context enrichment  
✅ **Execution Time Logging** - Built-in helpers for measuring operation duration  
✅ **Logging Scopes** - Strongly-typed property scopes for tenant, user, and operation context  
✅ **ASP.NET Core Middleware** - Track correlation IDs and request IDs across HTTP requests  
✅ **Testing Utilities** - In-memory sink and assertion helpers for unit tests  
✅ **.NET 10.0+** - Modern C# features and latest language version  
✅ **80%+ Test Coverage** - Comprehensive unit tests with FluentAssertions

---

## Installation

```bash
dotnet add package Clywell.Core.Logging
```

---

## Quick Start

### 1. Basic Setup (ASP.NET Core)

```csharp
using Clywell.Core.Logging.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddLogging();

var app = builder.Build();

app.UseRequestTracking(); // adds CorrelationId + RequestId middleware
app.UseRequestLogging();  // adds Serilog HTTP request logging

app.MapGet("/", (ILogger<Program> logger) =>
{
    logger.LogInformation("Hello from Clywell logging!");
    return "Hello World!";
});

app.Run();
```

`AddLogging()` with no arguments applies:
- `WithClywellDefaults()` — correlation ID, request ID, environment enrichers, sensitive data redaction
- `WithConsoleSink()` — human-readable console output
- `ReadFromConfiguration()` — merges `appsettings.json` Serilog section if present

### 2. Custom Configuration

```csharp
using Serilog.Events;

builder.AddLogging(config =>
{
    config
        .WithMinimumLevel(LogEventLevel.Debug)
        .WithConsoleSink(useJson: true)
        .WithFileSink("logs/app-.txt")
        .WithSeqSink("http://localhost:5341")
        .WithApplicationInsightsSink();
});
```

### 3. Manual Setup (Console App)

```csharp
using Clywell.Core.Logging.Configuration;
using Serilog.Events;

ClywellLoggerConfiguration.Create()
    .WithMinimumLevel(LogEventLevel.Information)
    .WithConsoleSink()
    .WithCorrelationId()
    .WithSensitiveDataRedaction()
    .BuildAndSetGlobalLogger();

Log.Information("Application started");
```

---

## Core Components

### 1. ApplicationBuilderExtensions

#### `AddLogging` — registers Serilog as the logging provider

```csharp
// Default setup
builder.AddLogging();

// With custom configuration
builder.AddLogging(config =>
{
    config
        .WithMinimumLevel(LogEventLevel.Debug)
        .WithConsoleSink()
        .WithClywellDefaults();
});
```

#### `UseRequestTracking` — adds correlation ID and request ID middleware

```csharp
app.UseRequestTracking();
```

Call this **early in the pipeline** so IDs are available for all logs in subsequent middleware.

#### `UseRequestLogging` — adds Serilog HTTP request logging

```csharp
app.UseRequestLogging();
```

Logs every HTTP request with method, path, status code, elapsed time, host, scheme, remote IP, user agent, correlation ID, and request ID.

---

### 2. ClywellLoggerConfiguration

Fluent builder for Serilog, used inside `AddLogging()` or standalone for console apps.

```csharp
ClywellLoggerConfiguration.Create()              // static factory; accepts optional IConfiguration
    .WithMinimumLevel(LogEventLevel.Information)
    .WithConsoleSink(useJson: false)             // false = human-readable, true = JSON
    .WithFileSink("logs/app-.txt", RollingInterval.Day)
    .WithSeqSink("http://localhost:5341")
    .WithApplicationInsightsSink(connectionString)
    .WithCorrelationId()                         // add CorrelationIdEnricher
    .WithRequestId()                             // add RequestIdEnricher
    .WithEnvironmentEnrichers()                  // MachineName + FromLogContext
    .WithSensitiveDataRedaction()                // SensitiveDataRedactionPolicy
    .WithClywellDefaults()                       // all 4 enrichers/policies in one call
    .OverrideMinimumLevel("Microsoft", LogEventLevel.Warning)
    .ReadFromConfiguration()                     // merge appsettings.json Serilog section
    .Build();                                    // returns Serilog ILogger

// Or set the global Serilog logger directly:
config.BuildAndSetGlobalLogger();
```

`WithClywellDefaults()` is shorthand for:
`WithCorrelationId()` + `WithRequestId()` + `WithEnvironmentEnrichers()` + `WithSensitiveDataRedaction()`

---

### 3. LoggerExtensions

Performance-optimized shorthand methods on `ILogger`. Every method includes a built-in `IsEnabled()` guard to prevent unnecessary string allocations.

```csharp
using Clywell.Core.Logging.Extensions;

logger.Trace("Entering {Method}", nameof(MyMethod));
logger.Debug("Processing item {ItemId}", itemId);
logger.Info("User {UserId} logged in", userId);
logger.Warning("Retry attempt {Attempt} for {Operation}", attempt, operation);
logger.Error("Failed to save order {OrderId}", orderId);
logger.Error(exception, "Unhandled error processing {OrderId}", orderId);
logger.Critical("Database unreachable");
logger.Critical(exception, "Fatal startup error");
```

These are drop-in replacements for the standard `LogTrace`, `LogDebug`, `LogInformation`, etc. with the automatic `IsEnabled` check built in.

#### Execution Time Logging

```csharp
// Synchronous
var result = logger.LogExecutionTime("DatabaseQuery", () => db.QueryData());
// Logs: DatabaseQuery completed in 45ms

// Asynchronous
var data = await logger.LogExecutionTimeAsync("ApiCall",
    async () => await httpClient.GetAsync("https://api.example.com"));
// Logs: ApiCall completed in 230ms
```

#### Timed Scopes

```csharp
using (logger.BeginTimedScope("ProcessOrder", new Dictionary<string, object>
{
    ["OrderId"] = orderId,
    ["CustomerId"] = customerId
}))
{
    // all logs here include the scope properties
}
// Logs: Starting ProcessOrder with properties { OrderId: 123, CustomerId: 456 }
// Logs: Completed ProcessOrder in 1234ms
```

---

### 4. LoggerScopeExtensions

Creates structured logging scopes. All log messages within a scope automatically carry its properties.

```csharp
using Clywell.Core.Logging.Extensions;

// Single property
using (logger.BeginPropertyScope("OrderId", orderId))
    logger.Info("Order processing started");

// Two or three properties (tuples)
using (logger.BeginPropertyScope(("TenantId", tenantId), ("UserId", userId)))
    logger.Info("User action performed");

// Dictionary of properties
using (logger.BeginPropertyScope(new Dictionary<string, object?>
{
    ["Environment"] = env,
    ["Version"] = version
}))
    logger.Info("Deployment started");

// Params tuples (any number)
using (logger.BeginPropertyScope(
    ("Region", region),
    ("Service", service),
    ("InstanceId", instanceId)))
    logger.Info("Service registered");
```

#### Convenience Scopes

```csharp
// Adds "TenantId" property
using (logger.BeginTenantScope(tenantId)) { ... }

// Adds "UserId" property
using (logger.BeginUserScope(userId)) { ... }

// Adds "TenantId" and "UserId" properties
using (logger.BeginTenantUserScope(tenantId, userId)) { ... }

// Adds "OperationName" (and optionally "OperationId") properties
using (logger.BeginOperationScope("PlaceOrder", operationId)) { ... }
```

---

### 5. Enrichers

#### CorrelationIdEnricher

Adds a `CorrelationId` property to every log entry.

- Uses `AsyncLocal<string?>` — scoped to the current async execution context
- If no value is set, a new GUID is generated automatically per log event
- The `CorrelationIdMiddleware` reads `X-Correlation-ID` from the request header (or generates a GUID), stores it on the enricher, writes it to the response header, and stores it in `HttpContext.Items["CorrelationId"]`

```csharp
using Clywell.Core.Logging.Enrichers;

// Set manually (e.g., in a background job or test)
CorrelationIdEnricher.CurrentCorrelationId = "my-correlation-id";
```

#### RequestIdEnricher

Adds a `RequestId` property to log entries when a value is set.

- Uses `AsyncLocal<string?>` — only enriches when a value is present (no fallback GUID)
- The `RequestIdMiddleware` reads `X-Request-ID` from the request header (or generates a GUID), stores it on the enricher, writes it to the response header, and stores it in `HttpContext.Items["RequestId"]`

```csharp
using Clywell.Core.Logging.Enrichers;

RequestIdEnricher.CurrentRequestId = "my-request-id";
```

---

### 6. Middleware

Both middleware components are registered together via `app.UseRequestTracking()`, or individually:

```csharp
app.UseMiddleware<CorrelationIdMiddleware>();
app.UseMiddleware<RequestIdMiddleware>();
```

| Middleware | Header read | Header written | `HttpContext.Items` key |
|---|---|---|---|
| `CorrelationIdMiddleware` | `X-Correlation-ID` | `X-Correlation-ID` | `"CorrelationId"` |
| `RequestIdMiddleware` | `X-Request-ID` | `X-Request-ID` | `"RequestId"` |

---

### 7. Sensitive Data Redaction

`SensitiveDataRedactionPolicy` is a Serilog `IDestructuringPolicy` that scans string log values and replaces matched patterns with `***REDACTED***`.

**Default patterns:**

| Pattern | Example input | Result |
|---|---|---|
| Credit card | `4532-1234-5678-9010` | `***REDACTED***` |
| SSN | `123-45-6789` | `***REDACTED***` |
| Password field | `password: secret` | `***REDACTED***` |
| API key field | `api_key: abc123` | `***REDACTED***` |
| Email + password combo | `user@example.com: secret` | `***REDACTED***` |

#### Enable via `ClywellLoggerConfiguration`

```csharp
config.WithSensitiveDataRedaction(); // included automatically in WithClywellDefaults()
```

#### Enable via dependency injection

```csharp
// Default — all built-in patterns
builder.Services.AddSensitiveDataRedaction();

// Custom configuration
builder.Services.AddSensitiveDataRedaction(options =>
{
    options
        .DisableCreditCardRedaction()
        .AddCustomPattern(@"\bINTERNAL_TOKEN\b");
});
```

#### `SensitiveDataRedactionPolicyOptions`

```csharp
SensitiveDataRedactionPolicyOptions.Create()
    .AddCustomPattern(@"\bDATABASE_URL\b")
    .AddCustomPattern(@"\bAPI_TOKEN\b", RegexOptions.None)
    .DisableCreditCardRedaction()
    .DisableSocialSecurityRedaction()
    .DisablePasswordRedaction()
    .DisableApiKeyRedaction()
    .DisableEmailPasswordRedaction()
    .DisableAllDefaults()
    .Build(); // returns SensitiveDataRedactionPolicy
```

#### Static utility

```csharp
var redacted = SensitiveDataRedactionPolicy.RedactSensitiveData("password: secret123");
// Returns: ***REDACTED***
```

---

### 8. Testing Utilities

The `Clywell.Core.Logging.Testing` namespace provides helpers for unit testing logging behaviour.

#### `InMemoryLogSink`

Captures Serilog log events during a test.

```csharp
var sink = new InMemoryLogSink();

sink.LoggedEvents                               // all captured events
sink.GetEvents(LogEventLevel.Warning)           // at or above a level
sink.GetEventsForLevel(LogEventLevel.Error)     // at exactly a level
sink.GetEventsContaining("OrderId")             // by message template text
sink.GetEventsWithProperty("TenantId")          // by property name
sink.Clear()                                    // reset between tests
```

#### `TestLoggerFactory`

Creates `Microsoft.Extensions.Logging.ILogger` instances backed by an `InMemoryLogSink`.

```csharp
// Logger + sink together (most common)
var (logger, sink) = TestLoggerFactory.CreateTestLoggerWithSink();

// Typed logger
var (logger, sink) = TestLoggerFactory.CreateTestLoggerWithSink<MyService>();

// Provide your own sink
var sink = new InMemoryLogSink();
var logger = TestLoggerFactory.CreateTestLogger(sink, LogEventLevel.Debug);
```

#### `LogAssertions`

Extension methods on `InMemoryLogSink` for test assertions.

```csharp
sink.ShouldHaveLogged(LogEventLevel.Information);
sink.ShouldHaveLogged(LogEventLevel.Error, "OrderId");
sink.ShouldNotHaveLogged(LogEventLevel.Warning);
sink.ShouldHaveProperty("CorrelationId");
sink.ShouldHavePropertyValue("TenantId", "acme");
sink.ShouldHaveException<InvalidOperationException>();
```

**Example test:**

```csharp
[Fact]
public void LogsUserIdInScope()
{
    var (logger, sink) = TestLoggerFactory.CreateTestLoggerWithSink<UserService>();

    using (logger.BeginUserScope("user-123"))
    {
        logger.Info("Action performed");
    }

    sink.ShouldHaveLogged(LogEventLevel.Information, "Action performed");
    sink.ShouldHavePropertyValue("UserId", "user-123");
}
```

---

## appsettings.json Configuration

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "System": "Warning"
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj} {Properties:j}{NewLine}{Exception}"
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/app-.txt",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 31
        }
      }
    ],
    "Enrich": ["FromLogContext", "WithMachineName"]
  },
  "Logging": {
    "Seq": {
      "ServerUrl": "http://localhost:5341"
    }
  },
  "ApplicationInsights": {
    "ConnectionString": "InstrumentationKey=..."
  }
}
```

`ReadFromConfiguration()` is called automatically by `AddLogging()`, or explicitly:

```csharp
builder.AddLogging(config => config.ReadFromConfiguration().WithClywellDefaults());
```

---

## Environment-Specific Examples

### Development

```csharp
builder.AddLogging(config =>
{
    config
        .WithMinimumLevel(LogEventLevel.Debug)
        .WithConsoleSink()
        .WithFileSink("logs/dev-.txt")
        .WithClywellDefaults();
});
```

### Production

```csharp
builder.AddLogging(config =>
{
    config
        .WithMinimumLevel(LogEventLevel.Information)
        .WithConsoleSink(useJson: true)
        .WithApplicationInsightsSink()
        .WithClywellDefaults()
        .OverrideMinimumLevel("Microsoft", LogEventLevel.Warning)
        .OverrideMinimumLevel("System", LogEventLevel.Warning);
});
```

---

## Middleware Pipeline Order

```csharp
var app = builder.Build();

app.UseRequestTracking(); // FIRST — sets CorrelationId and RequestId for all logs below
app.UseRequestLogging();  // SECOND — logs each request with enriched properties

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
```

---

## Best Practices

### Use structured logging

```csharp
// ❌ BAD: string interpolation loses structure
logger.LogInformation($"User {userId} logged in");

// ✅ GOOD: named placeholder preserves structure
logger.LogInformation("User {UserId} logged in", userId);
```

### Use shorthand extensions for expensive log values

```csharp
// ❌ BAD: serialization always runs even if Debug is disabled
logger.LogDebug($"Query result: {JsonSerializer.Serialize(largeObject)}");

// ✅ GOOD: Debug() checks IsEnabled first; serialization only runs when needed
logger.Debug("Query result: {Result}", JsonSerializer.Serialize(largeObject));
```

### Use execution time helpers instead of manual stopwatches

```csharp
// ❌ BAD: boilerplate
var sw = Stopwatch.StartNew();
var result = await database.QueryAsync();
sw.Stop();
logger.LogInformation("Query took {ElapsedMs}ms", sw.ElapsedMilliseconds);

// ✅ GOOD
var result = await logger.LogExecutionTimeAsync("DatabaseQuery", () => database.QueryAsync());
```

### Use scopes to correlate log groups

```csharp
// All logs inside this scope automatically include TenantId and UserId
using (logger.BeginTenantUserScope(tenantId, userId))
{
    logger.Info("Processing request");
    await ProcessAsync();
    logger.Info("Request complete");
}
```

---

## Troubleshooting

### Logs Not Appearing

1. Check log level configuration
2. Ensure middleware is added to the pipeline
3. Verify sinks are configured correctly

```csharp
// Debug: Enable verbose logging temporarily
config.WithMinimumLevel(LogEventLevel.Verbose);
```

### Correlation IDs Not Working

Ensure middleware is added **early** in the pipeline:

```csharp
app.UseRequestTracking(); // Must be early
app.UseRouting();
app.UseAuthorization();
app.MapControllers();
```

And confirm the enricher is registered:

```csharp
config.WithCorrelationId();
// Or use defaults:
config.WithClywellDefaults();
```

### High Memory Usage

Reduce log verbosity in production and rely on the built-in `IsEnabled` guards:

```csharp
config
    .WithMinimumLevel(LogEventLevel.Information)
    .OverrideMinimumLevel("Microsoft", LogEventLevel.Warning)
    .OverrideMinimumLevel("System", LogEventLevel.Warning);

// Shorthand extensions check IsEnabled — no allocation when level is disabled
logger.Debug("Result: {Data}", GetExpensiveData());
```

---

## Requirements

- **.NET 10.0+**
- **Serilog 4.0+**
- **ASP.NET Core 10.0+** (for `AddLogging`, `UseRequestTracking`, `UseRequestLogging`)

---

## Dependencies

- `Serilog`
- `Serilog.AspNetCore`
- `Serilog.Sinks.Console`
- `Serilog.Sinks.File`
- `Serilog.Sinks.Seq`
- `Serilog.Sinks.ApplicationInsights`
- `Serilog.Enrichers.Environment`
- `Serilog.Enrichers.Thread`
- `Microsoft.Extensions.Logging.Abstractions`

---

## License

MIT License - See [LICENSE](LICENSE) file

---

## Contributing

This is an infrastructure package with **no business logic**. Contributions are welcome!

1. Fork the repository
2. Create a feature branch
3. Make your changes with tests
4. Ensure 80%+ code coverage
5. Submit a pull request

---

## Support

For issues and questions, open a [GitHub Issue](https://github.com/clywell/clywell-logging/issues).

---

**Built with ❤️ by the Clywell team**