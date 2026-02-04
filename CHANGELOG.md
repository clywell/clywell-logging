# Changelog

All notable changes to Clywell.Core.Logging will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [Unreleased]

### Added
- Initial release of Clywell.Core.Logging
- Serilog integration with fluent configuration API
- Correlation ID enricher for distributed tracing
- Request ID enricher for HTTP request tracking
- Sensitive data redaction policy (credit cards, SSNs, passwords, API keys)
- Correlation ID middleware for ASP.NET Core
- Request ID middleware for ASP.NET Core
- Performance-optimized logging extensions (`LogDebugIfEnabled`, `LogTraceIfEnabled`)
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

## [1.0.0-alpha.1] - 2026-02-03

### Added
- First alpha release for internal testing
- Core logging infrastructure complete
- All planned features implemented
- Tests passing with 80%+ coverage

---

[Unreleased]: https://github.com/clywell/clywell-logging/compare/v1.0.0-alpha.1...HEAD
[1.0.0-alpha.1]: https://github.com/clywell/clywell-logging/releases/tag/v1.0.0-alpha.1
