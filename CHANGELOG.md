# Changelog

All notable changes to Clywell.Core.Logging will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [1.1.1] - 2026-04-18

### Changed

#### `Clywell.Core.Logging`
- Bumped `Serilog.Sinks.ApplicationInsights` from 5.0.0 to 5.0.1
- Bumped `Microsoft.Extensions.Logging.Abstractions` from 10.0.3 to 10.0.6
- Bumped `Microsoft.Extensions.Configuration.Abstractions` from 10.0.3 to 10.0.6
- Bumped `Microsoft.Extensions.DependencyInjection.Abstractions` from 10.0.3 to 10.0.6
- Bumped `Microsoft.NET.Test.Sdk` from 18.3.0 to 18.4.0 (test-only)
- Bumped `coverlet.collector` from 8.0.0 to 10.0.0 (test-only)
- Bumped `Microsoft.AspNetCore.OpenApi` from 10.0.0 to 10.0.3

---

## [1.1.0] - 2026-03-08

### Fixed
- `AddLogging` now uses `builder.Host.UseSerilog()` instead of `builder.Logging.AddSerilog()` to correctly register `DiagnosticContext` in the DI container, enabling `UseRequestLogging()` (`UseSerilogRequestLogging`) to work without a startup exception.

---

## [1.0.0] - 2026-02-25

### Added
- Initial release of Clywell.Core.Logging
- Serilog integration with fluent configuration API
- Correlation ID enricher for distributed tracing
- Request ID enricher for HTTP request tracking
- Sensitive data redaction policy (credit cards, SSNs, passwords, API keys)
- Correlation ID middleware for ASP.NET Core
- Request ID middleware for ASP.NET Core
- Performance-optimized shorthand logging extensions (`Debug`, `Trace`, `Info`, `Warning`, `Error`, `Critical`) with built-in `IsEnabled()` guards
- Execution time logging helpers (sync and async)
- Timed logging scopes
- ClywellLoggerConfiguration fluent builder
- Support for multiple sinks: Console, File, Seq, Application Insights
- Environment enrichers (machine name, thread info)
- Log level override support
- Configuration reading from appsettings.json
- ASP.NET Core integration extensions
- Comprehensive unit tests (80%+ coverage)
- README with examples and best practices
- MIT License

### Changed
- N/A

### Deprecated
- N/A

### Removed
- N/A

### Fixed
- N/A

### Security
- Sensitive data redaction prevents logging of:
  - Credit card numbers
  - Social Security Numbers
  - Passwords
  - API keys and access tokens
  - Email/password combinations

---

[Unreleased]: https://github.com/clywell/clywell-logging/compare/v1.1.0...HEAD
[1.1.0]: https://github.com/clywell/clywell-logging/compare/v1.0.0...v1.1.0
[1.0.0]: https://github.com/clywell/clywell-logging/releases/tag/v1.0.0
