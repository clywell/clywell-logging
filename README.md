# Clywell.Core.Logging

[![NuGet](https://img.shields.io/nuget/v/Clywell.Core.Logging.svg)](https://www.nuget.org/packages/Clywell.Core.Logging/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

Structured logging infrastructure with Serilog for the Clywell platform. Provides log enrichers, configuration helpers, and performance-optimized logging utilities with **zero business logic** - can be used in any .NET application.

---

## Features

✅ **Serilog Integration** - Fluent configuration for Serilog with best practices  
✅ **Correlation & Request Tracking** - Automatic correlation IDs and request IDs for distributed tracing  
✅ **Performance Optimized** - `IsEnabled()` checks prevent unnecessary string allocations  
✅ **Sensitive Data Redaction** - Automatic redaction of passwords, credit cards, API keys  
✅ **Multiple Sinks** - Console, File, Seq, Application Insights  
✅ **Environment Enrichers** - Machine name, thread info, custom properties  
✅ **Execution Time Logging** - Built-in helpers for measuring operation duration  
✅ **ASP.NET Core Middleware** - Track correlation IDs and request IDs across HTTP requests  
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

// Add Clywell logging with defaults
// Note: This configures Serilog and sets Log.Logger
builder.AddLogging();

var app = builder.Build();

// Add correlation ID and request ID tracking
// Must be called before RequestLogging
app.UseRequestTracking();

// Add Serilog request logging
app.UseRequestLogging();

app.MapGet("/", (ILogger<Program> logger) =>
{
    // Use Clywell extension for IsEnabled check
    logger.Info("Hello from Clywell logging!");
    return "Hello World!";
});

app.Run();
```

### 2. Custom Configuration

```csharp
builder.AddLogging(config =>
{
    config
        .WithMinimumLevel(LogEventLevel.Debug)
        .WithConsoleSink(useJson: true)
        .WithFileSink("logs/app-.txt")
        .WithSeqSink("http://localhost:5341")
        .WithApplicationInsightsSink()
        .WithClywellDefaults(); // Adds all enrichers
});
```

### 3. Manual Setup (Console App)

```csharp
using Clywell.Core.Logging.Configuration;
using Serilog;

var logger = ClywellLoggerConfiguration.Create()
    .WithMinimumLevel(LogEventLevel.Information)
    .WithConsoleSink()
    .WithCorrelationId()
    .WithSensitiveDataRedaction()
    .Build();

Log.Logger = logger;

Log.Information("Application started");
```

---

## Core Components

### 1. Enrichers

#### Correlation ID Enricher

Adds a correlation ID to every log entry for distributed tracing:

```csharp
using Clywell.Core.Logging.Enrichers;

// Set correlation ID manually
CorrelationIdEnricher.CurrentCorrelationId = "custom-correlation-id";

logger.LogInformation("This log will have the correlation ID");

// Or use middleware (automatic)
app.UseRequestTracking();
```

#### Request ID Enricher

Adds a unique request ID to every log entry:

```csharp
using Clywell.Core.Logging.Enrichers;

// Set request ID manually
RequestIdEnricher.CurrentRequestId = "custom-request-id";

logger.LogInformation("This log will have the request ID");

// Or use middleware (automatic)
app.UseClywellRequestTracking();
```

### 2. Sensitive Data Redaction

Automatically redacts sensitive data from logs:

```csharp
logger.LogInformation("User password: {Password}", "mySecret123");
// Output: User password: ***REDACTED***

logger.LogInformation("Card: {CardNumber}", "4532-1234-5678-9010");
// Output: Card: ***REDACTED***
```

**Supported patterns:**
- Credit cards (4532-1234-5678-9010)
- Social Security Numbers (123-45-6789)
- Passwords (password: secret)
- API Keys (api_key: abc123)
- Email/password combos

### 3. Performance Logging Extensions

#### Debug/Trace with IsEnabled Check

```csharp
using Clywell.Core.Logging.Extensions;

// Only logs if Debug is enabled (internal check)
logger.Debug("Expensive operation: {Data}", expensiveData);

// Only logs if Trace is enabled (internal check)
logger.Trace("Very detailed trace: {Data}", traceData);
```

#### Execution Time Logging

```csharp
// Sync operation
var result = logger.LogExecutionTime("DatabaseQuery", () =>
{
    return database.QueryData();
});
// Output: DatabaseQuery completed in 45ms

// Async operation
var result = await logger.LogExecutionTimeAsync("ApiCall", async () =>
{
    return await httpClient.GetAsync("https://api.example.com");
});
// Output: ApiCall completed in 230ms
```

#### Timed Scopes

```csharp
using (logger.BeginTimedScope("ProcessOrder", new Dictionary<string, object>
{
    ["OrderId"] = orderId,
    ["CustomerId"] = customerId
}))
{
    // Process order
}
// Output: Starting ProcessOrder with properties { OrderId: 123, CustomerId: 456 }
// Output: Completed ProcessOrder in 1234ms
```

### 4. Middleware

#### Correlation ID Middleware

```csharp
app.UseMiddleware<CorrelationIdMiddleware>();

// Or use the extension
app.UseClywellRequestTracking();
```

**Features:**
- Reads `X-Correlation-ID` header from request
- Generates new GUID if not provided
- Adds `X-Correlation-ID` to response headers
- Stores in `HttpContext.Items["CorrelationId"]`
- Automatically enriches all logs

#### Request ID Middleware

```csharp
app.UseMiddleware<RequestIdMiddleware>();

// Or use the extension
app.UseClywellRequestTracking();
```

**Features:**
- Reads `X-Request-ID` header from request
- Generates new GUID if not provided
- Adds `X-Request-ID` to response headers
- Stores in `HttpContext.Items["RequestId"]`
- Automatically enriches all logs

---

## Configuration Examples

### Development Environment

```csharp
builder.AddLogging(config =>
{
    config
        .WithMinimumLevel(LogEventLevel.Debug)
        .WithConsoleSink() // Human-readable
        .WithFileSink("logs/dev-.txt")
        .WithClywellDefaults();
});
```

### Production Environment

```csharp
builder.AddLogging(config =>
{
    config
        .WithMinimumLevel(LogEventLevel.Information)
        .WithConsoleSink(useJson: true) // JSON for container logs
        .WithApplicationInsightsSink() // Azure monitoring
        .WithClywellDefaults()
        .OverrideMinimumLevel("Microsoft", LogEventLevel.Warning) // Reduce noise
        .OverrideMinimumLevel("System", LogEventLevel.Warning);
});
```

### Full Configuration

```csharp
var logger = ClywellLoggerConfiguration.Create(configuration)
    .WithMinimumLevel(LogEventLevel.Debug)
    .WithConsoleSink(useJson: false)
    .WithFileSink("logs/app-.txt", RollingInterval.Day)
    .WithSeqSink("http://localhost:5341")
    .WithApplicationInsightsSink(connectionString)
    .WithCorrelationId()
    .WithRequestId()
    .WithEnvironmentEnrichers()
    .WithSensitiveDataRedaction()
    .OverrideMinimumLevel("Microsoft.EntityFrameworkCore", LogEventLevel.Warning)
    .Build();

Log.Logger = logger;
```

---

## appsettings.json Configuration

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    },
    "Seq": {
      "ServerUrl": "http://localhost:5341"
    }
  },
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
  "ApplicationInsights": {
    "ConnectionString": "InstrumentationKey=..."
  }
}
```

Then read from configuration:

```csharp
builder.AddLogging(config =>
{
    config.ReadFromConfiguration()
          .WithClywellDefaults();
});
```

---

## Testing

The package includes comprehensive unit tests with 80%+ code coverage:

```bash
dotnet test
```

**Test coverage:**
- ✅ Correlation ID enricher
- ✅ Request ID enricher
- ✅ Sensitive data redaction
- ✅ Configuration builder
- ✅ Performance logging extensions
- ✅ Middleware (correlation & request IDs)

---

## Best Practices

### 1. Use Structured Logging

```csharp
// ❌ BAD: String interpolation
logger.LogInformation($"User {userId} logged in");

// ✅ GOOD: Structured logging
logger.LogInformation("User {UserId} logged in", userId);
```

### 2. Use Performance Extensions for Expensive Logs

```csharp
// ❌ BAD: Always creates the string
logger.LogDebug($"Query result: {JsonSerializer.Serialize(largeObject)}");

// ✅ GOOD: Only creates string if Debug is enabled (internal check)
logger.Debug("Query result: {Result}", JsonSerializer.Serialize(largeObject));
```

### 3. Use Execution Time Logging

```csharp
// ❌ BAD: Manual timing
var stopwatch = Stopwatch.StartNew();
var result = await database.QueryAsync();
stopwatch.Stop();
logger.LogInformation("Query took {ElapsedMs}ms", stopwatch.ElapsedMilliseconds);

// ✅ GOOD: Built-in timing
var result = await logger.LogExecutionTimeAsync("DatabaseQuery", 
    () => database.QueryAsync());
```

### 4. Always Use Middleware

```csharp
// Required for correlation/request IDs
app.UseRequestTracking();
app.UseRequestLogging();
```

---

## Requirements

- **.NET 10.0+**
- **Serilog 4.0+**
- **ASP.NET Core 10.0+** (for middleware)

---

## Dependencies

- Serilog
- Serilog.AspNetCore
- Serilog.Sinks.Console
- Serilog.Sinks.File
- Serilog.Sinks.Seq
- Serilog.Sinks.ApplicationInsights
- Microsoft.Extensions.Logging.Abstractions

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

For issues and questions:
- GitHub Issues: [clywell/clywell-logging](https://github.com/clywell/clywell-logging/issues)
- Documentation: [docs/getting-started.md](docs/getting-started.md)

---

## Roadmap

- [ ] Add structured log viewer UI
- [ ] Support for custom enrichers via configuration
- [ ] Integration with OpenTelemetry
- [ ] Performance benchmarks
- [ ] More sink integrations (Elasticsearch, Datadog)

---

**Built with ❤️ by the Clywell team**
