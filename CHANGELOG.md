# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.6.0] - 2025-01-XX

### ?? Added - Metrics and Global Cancellation

- **?? Real-time metrics system** with `LoaderMetrics`
  - `TotalRequests`, `SuccessfulRequests`, `FailedRequests` counters
  - `RetriedRequests`, `TotalRetryAttempts` retry statistics
  - `AverageResponseTime`, `MaxResponseTime`, `MinResponseTime` performance metrics
  - `StatusCodeDistribution` for HTTP code analysis
  - `SuccessRate`, `FailureRate`, `RetryRate` percentages
  - `LastRequestTime` timestamp tracking

- **?? Global CancellationToken support**
  - `GlobalCancellationToken` property on `LoaderService`
 - `CancelAllRequests()` method to cancel all ongoing requests
  - Automatic integration with HTTP handler
  - Support for manual usage in custom code

- **??? New LoaderService methods**
  - `ResetMetrics()` - Reset all metrics to zero
  - `GetMetricsSummary()` - Get formatted metrics summary

### ?? Documentation

- New `docs/METRICS_AND_CANCELLATION.md` - Complete guide with examples
- New `docs/UPGRADE_GUIDE_V1.6.md` - Migration guide from v1.5.0
- New `Examples/MetricsAndCancellationExamples.cs` - 9 usage examples
- Updated README with metrics and cancellation features

### ?? Technical

- Created `Models/LoaderMetrics.cs` class
- Thread-safe metrics collection with `ConcurrentDictionary` and `ConcurrentQueue`
- Automatic metrics recording in `HttpCallInterceptorHandler`
- Linked cancellation tokens for global cancellation support

### ? Backward Compatibility

- 100% compatible with v1.5.0
- No configuration changes required
- Metrics automatically enabled and collected
- Existing code continues to work without modification

## [1.5.0] - 2025-01-XX

### ?? Added - Advanced HTTP Interceptor Configuration

- **?? Advanced retry configuration** with `HttpInterceptorOptions`
  - `MaxRetryAttempts` : Configurable number of attempts (default: 3)
  - `RetryDelay` : Configurable delay between attempts (default: 1s)
  - `UseExponentialBackoff` : Exponential backoff support (default: true)
  - `RetryOnStatusCodes` : Customizable HTTP codes triggering retry
  - `RetryOnTimeout` : Option to retry on timeout

- **?? Conditional request filtering**
  - `InterceptPredicate` : Filter requests to intercept (e.g., only `/api/*`)
  - `ShowLoaderPredicate` : Filter loader display (e.g., only POST/PUT/DELETE)

- **?? Integrated logging with ILogger**
  - Automatic HTTP request logs (start, end, duration, status code)
  - Retry attempt logs
  - Error logs after all attempts exhausted
  - Detailed logging support with `EnableDetailedLogging`

- **?? Customizable callbacks**
  - `OnRetry` : Callback invoked before each retry attempt
  - Enables notifications, custom logging, etc.

- **?? Complete documentation**
  - New `docs/ADVANCED_CONFIGURATION.md` file with detailed examples
  - Updated README with all new options
  - Practical examples for each feature

### ?? Enhanced

- **?? Smarter retry logic**
  - Exponential backoff by default (1s, 2s, 4s, 8s...)
  - Support for multiple HTTP error codes (500, 502, 503, 504, 408)
  - Timeout handling with automatic retry

- **? Performance**
  - Conditional logging to avoid overhead in production
  - Request filtering to reduce performance impact

- **?? Backward compatibility**
  - All existing methods continue to work
  - Default options match previous behavior
  - Transparent migration without breaking changes

### ?? Technical

- Created `HttpInterceptorOptions` class for configuration
- Refactored `HttpCallInterceptorHandler` to support options
- Added overloads in `ServiceCollectionExtensions`
- Support for `ILogger<HttpCallInterceptorHandler>` injection

### ?? Examples

```csharp
// Basic upgrade - no changes required
builder.Services.AddBlazorFlexLoaderWithHttpInterceptor(client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

// New advanced configuration
builder.Services.AddBlazorFlexLoaderWithHttpInterceptor(
    client =>
    {
        client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    },
    options =>
    {
 options.MaxRetryAttempts = 5;
        options.UseExponentialBackoff = true;
        options.InterceptPredicate = request => 
          request.RequestUri?.AbsolutePath.StartsWith("/api/") ?? false;
    });
```

## [1.4.0] - 2024-10-21

### ?? Added - Built-in Animated SVG Loader
- ? **Default animated SVG loader** when no ImagePath is specified
- ?? **Professional Blazor-themed design** with official color palette
- ?? **Smooth CSS animations** with rotating rings and pulsing dots
- ?? **Zero configuration required** - just add `<FlexLoader />`
- ?? **Optimized 80x80px size** for perfect visibility
- ??? **Maintains full customization** options for advanced users

### Enhanced
- ?? **DefaultLoaderStyle** property for SVG styling
- ?? **Updated documentation** with default loader examples
- ??? **New package tags** including 'svg' and 'animated'
- ?? **README highlights** the built-in SVG feature

### Technical
- ?? **Embedded SVG** with gradients and animations
- ?? **Conditional rendering** - SVG when no image, image when specified
- ?? **CSS animations** for smooth performance
- ?? **No external dependencies** - everything embedded

## [1.3.4] - 2024-10-21

### Fixed
- ?? **README encoding issues** resolved
- ?? **ASCII-compatible documentation** for universal display
- ?? **Cross-platform compatibility** improved

## [1.3.3] - 2024-10-21

### Added
- ?? **Professional branding assets** with SVG icons
- ? **Animated icon** for documentation and web display
- ?? **Static icon** optimized for package managers
- ?? **Favicon** for web applications
- ?? **Complete CI/CD pipeline** with GitHub Actions
- ?? **Comprehensive assets documentation**
- ?? **Automated dependency updates**

### Enhanced
- ?? **Visual identity** with Blazor-inspired design
- ?? **README** with centered animated icon
- ??? **Package tags** for better discoverability
- ?? **Release notes** with detailed information

### Technical
- ?? **SVG icons** with CSS animations and gradients
- ?? **128x128px** optimal sizing for package managers
- ?? **Color palette** aligned with Blazor branding
- ?? **PNG conversion instructions** for NuGet compatibility

## [1.2.0] - 2024-10-21

### Added
- ?? **Bilingual documentation** (French/English)
- ?? **Comprehensive XML documentation** with code examples
- ?? **Detailed usage guide** in `docs/LoaderService-Documentation.md`
- ?? **Best practices** and advanced usage examples
- ?? **Bilingual README** with navigation
- ??? **Package tags** for better discoverability

### Changed
- ?? **Enhanced package description** in both languages
- ?? **Updated author** to `daniwxcode`
- ?? **Documentation files** now included in NuGet package

### Technical
- ? **XML documentation** for all public methods
- ?? **Cross-references** between related methods
- ?? **Warning documentation** for critical methods
- ?? **IntelliSense support** improved

## [1.1.0] - 2024-10-21

### Added
- ? **New `Show()` method** - Simple alias for `Increment()`
- ? **New `Close()` method** - Simple alias for `Decrement()`
- ?? **XML documentation** for all methods
- ?? **Backward compatibility** maintained

### Changed
- ?? **Updated README** with new usage examples
- ?? **Simplified API** for common use cases

## [1.0.0] - 2024-10-21

### Added
- ?? **Initial release** of Blazor.FlexLoader
- ?? **LoaderService** with thread-safe counter
- ?? **FlexLoader component** with customizable UI
- ??? **Support for custom images** (GIF, SVG, PNG, etc.)
- ?? **Custom text** support
- ?? **Custom content** with RenderFragment
- ?? **Flexible positioning** (absolute/fixed)
- ?? **Customizable colors** and styles
- ? **Auto-close** functionality
- ?? **Click-to-close** overlay option
- ?? **Service collection extensions** for easy setup
- ?? **NuGet package** configuration
- ?? **MIT License**
- ?? **GitHub repository** setup

### Technical
- ?? **Targets .NET 9**
- ?? **Thread-safe operations** with `Interlocked`
- ?? **Event-driven** state management
- ?? **Comprehensive examples** and documentation

[1.6.0]: https://github.com/daniwxcode/Blazor.FlexLoader/compare/v1.5.0...v1.6.0
[1.5.0]: https://github.com/daniwxcode/Blazor.FlexLoader/compare/v1.4.0...v1.5.0
[1.4.0]: https://github.com/daniwxcode/Blazor.FlexLoader/compare/v1.3.4...v1.4.0
[1.3.4]: https://github.com/daniwxcode/Blazor.FlexLoader/compare/v1.3.3...v1.3.4
[1.3.3]: https://github.com/daniwxcode/Blazor.FlexLoader/compare/v1.2.0...v1.3.3
[1.2.0]: https://github.com/daniwxcode/Blazor.FlexLoader/compare/v1.1.0...v1.2.0
[1.1.0]: https://github.com/daniwxcode/Blazor.FlexLoader/compare/v1.0.0...v1.1.0
[1.0.0]: https://github.com/daniwxcode/Blazor.FlexLoader/releases/tag/v1.0.0