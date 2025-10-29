# Blazor.FlexLoader

[![CI](https://github.com/daniwxcode/Blazor.FlexLoader/actions/workflows/ci.yml/badge.svg)](https://github.com/daniwxcode/Blazor.FlexLoader/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Blazor.FlexLoader.svg)](https://www.nuget.org/packages/Blazor.FlexLoader/)
[![Downloads](https://img.shields.io/nuget/dt/Blazor.FlexLoader.svg)](https://www.nuget.org/packages/Blazor.FlexLoader/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

<div align="center">
  <img src="./assets/icon-github.svg" alt="Blazor.FlexLoader" width="128" height="128" />
  <h3>A flexible Blazor component for loading indicators</h3>
</div>

## Features

- **Built-in animated SVG** - Zero configuration required
- **Automatic HTTP interception** - Shows loader during HTTP requests
- **Automatic retry** - Retries failed requests with exponential backoff
- **Real-time metrics** - Track requests, success/failure rates, response times
- **Global cancellation** - Cancel all ongoing requests instantly
- **Advanced configuration** - Customizable retry, interception, metrics
- **Integrated logging** - Full traceability with ILogger
- **Conditional filtering** - Intercept only specific routes or methods
- Support for custom images (GIF, SVG, PNG)
- Customizable text and styles
- Custom content with RenderFragment
- Compatible with .NET 9

## Installation

```bash
dotnet add package Blazor.FlexLoader
```

## Quick Start

### 1. Register services in `Program.cs`

```csharp
using Blazor.FlexLoader.Extensions;

// Option A: Basic
builder.Services.AddBlazorFlexLoader();

// Option B: With HTTP interception (Recommended)
builder.Services.AddBlazorFlexLoaderWithHttpInterceptor(client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});
```

### 2. Add imports in `_Imports.razor`

```razor
@using Blazor.FlexLoader.Components
@using Blazor.FlexLoader.Services
```

### 3. Add component to layout

```razor
<FlexLoader />
```

## Usage

### Automatic (with HTTP interception)

```csharp
@inject IHttpClientFactory HttpClientFactory

private async Task FetchData()
{
    var client = HttpClientFactory.CreateClient("BlazorFlexLoader");
    var response = await client.GetAsync("/api/data");
  // Loader shows/hides automatically with retry on errors
}
```

### Manual

```csharp
@inject LoaderService LoaderService

private async Task DoWork()
{
    LoaderService.Show();
    try
    {
        await SomeAsyncOperation();
    }
    finally
    {
LoaderService.Close();
    }
}
```

## Advanced Configuration

```csharp
builder.Services.AddBlazorFlexLoaderWithHttpInterceptor(
    client =>
    {
        client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    },
options =>
    {
    // Retry configuration
  options.MaxRetryAttempts = 5;
        options.UseExponentialBackoff = true;
        options.RetryDelay = TimeSpan.FromSeconds(1);
        
        // Intercept only API routes
        options.InterceptPredicate = request => 
            request.RequestUri?.AbsolutePath.StartsWith("/api/") ?? false;
 
        // Show loader only for mutations
 options.ShowLoaderPredicate = request => 
request.Method != HttpMethod.Get;
        
        // Enable metrics
        options.EnableMetrics = true;
    
    // Callback on retry
        options.OnRetry = async (attempt, exception, delay) =>
        {
            Console.WriteLine($"Retry {attempt} after {delay.TotalSeconds}s");
  };
    });
```

## Metrics

Access real-time HTTP request metrics:

```csharp
@inject LoaderService LoaderService

var metrics = LoaderService.Metrics;

// Total requests
var total = metrics.TotalRequests;

// Success/failure rates
var successRate = metrics.SuccessRate;
var failureRate = metrics.FailureRate;

// Average response time
var avgTime = metrics.AverageResponseTime;

// HTTP status distribution
var statusCodes = metrics.StatusCodeDistribution;
```

## Cancellation

Cancel all ongoing HTTP requests:

```csharp
@inject LoaderService LoaderService

// Cancel all requests
LoaderService.CancelAllRequests();

// Get cancellation token for custom operations
var token = LoaderService.GetCancellationToken();
await CustomOperation(token);
```

## Component Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ImagePath` | `string?` | `null` | Path to custom image (null = SVG) |
| `Text` | `string?` | `"Loading..."` | Loading text |
| `BackgroundColor` | `string` | `"rgba(255,255,255,0.75)"` | Overlay background |
| `TextColor` | `string` | `"#333"` | Text color |
| `CustomContent` | `RenderFragment?` | `null` | Custom content |
| `AutoClose` | `bool` | `true` | Auto-close with delay |
| `CloseOnOverlayClick` | `bool` | `false` | Close on overlay click |

## Configuration Options

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `MaxRetryAttempts` | `int` | `3` | Max retry attempts |
| `RetryDelay` | `TimeSpan` | `1s` | Base delay between retries |
| `UseExponentialBackoff` | `bool` | `true` | Exponential delay increase |
| `RetryOnTimeout` | `bool` | `true` | Retry on timeout |
| `EnableMetrics` | `bool` | `true` | Collect performance metrics |
| `InterceptPredicate` | `Func` | `null` | Filter intercepted requests |
| `ShowLoaderPredicate` | `Func` | `null` | Filter loader display |
| `OnRetry` | `Func` | `null` | Callback before retry |

## Documentation

- [Advanced Configuration](./docs/ADVANCED_CONFIGURATION.md)
- [Metrics & Cancellation](./docs/METRICS_AND_CANCELLATION.md)
- [Upgrade Guide v1.6](./docs/UPGRADE_GUIDE_V1.6.md)
- [Service Documentation](./docs/LoaderService-Documentation.md)
- [Deployment Guide](./DEPLOYMENT.md)
- [Changelog](./CHANGELOG.md)

## Examples

See [Examples folder](./Examples/) for detailed usage patterns.

## License

MIT