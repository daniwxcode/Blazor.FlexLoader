# Métriques et CancellationToken Global - Guide d'utilisation

## ?? Métriques en temps réel

### Vue d'ensemble

Blazor.FlexLoader v1.6.0 inclut un système de métriques en temps réel qui vous permet de surveiller les performances de vos requêtes HTTP.

### Accès aux métriques

```csharp
@inject LoaderService LoaderService

var metrics = LoaderService.Metrics;
```

### Propriétés disponibles

| Propriété | Type | Description |
|-----------|------|-------------|
| `TotalRequests` | `int` | Nombre total de requêtes |
| `SuccessfulRequests` | `int` | Nombre de requêtes réussies (2xx) |
| `FailedRequests` | `int` | Nombre de requêtes échouées (4xx, 5xx) |
| `RetriedRequests` | `int` | Nombre de requêtes ayant nécessité un retry |
| `TotalRetryAttempts` | `int` | Nombre total de tentatives de retry |
| `AverageResponseTime` | `TimeSpan` | Temps de réponse moyen |
| `MaxResponseTime` | `TimeSpan` | Temps de réponse maximum |
| `MinResponseTime` | `TimeSpan` | Temps de réponse minimum |
| `StatusCodeDistribution` | `IReadOnlyDictionary<int, int>` | Distribution des codes HTTP |
| `LastRequestTime` | `DateTime?` | Date/heure de la dernière requête |
| `SuccessRate` | `double` | Taux de réussite (%) |
| `FailureRate` | `double` | Taux d'échec (%) |
| `RetryRate` | `double` | Taux de retry (%) |

## ?? Exemples d'utilisation

### 1. Affichage simple des métriques

```csharp
@inject LoaderService LoaderService

<div class="metrics">
    <p>Total Requests: @LoaderService.Metrics.TotalRequests</p>
    <p>Success Rate: @LoaderService.Metrics.SuccessRate.ToString("F2")%</p>
    <p>Avg Response: @LoaderService.Metrics.AverageResponseTime.TotalMilliseconds.ToString("F0")ms</p>
</div>
```

### 2. Résumé formaté

```csharp
Console.WriteLine(LoaderService.GetMetricsSummary());

/* Output:
Blazor.FlexLoader Metrics:
Total Requests: 150
Success Rate: 94.67%
Failure Rate: 5.33%
Retry Rate: 12.00%
Avg Response Time: 245.50ms
Max Response Time: 1250.00ms
Min Response Time: 85.00ms
Total Retries: 18
Last Request: 2025-01-15 14:30:25
*/
```

### 3. Dashboard de monitoring

```razor
@page "/metrics"
@inject LoaderService LoaderService
@implements IDisposable

<h3>?? Performance Dashboard</h3>

<div class="metrics-grid">
    <div class="metric-card @GetSuccessClass()">
      <div class="metric-value">@LoaderService.Metrics.SuccessRate.ToString("F1")%</div>
        <div class="metric-label">Success Rate</div>
    </div>
    
    <div class="metric-card">
        <div class="metric-value">@LoaderService.Metrics.AverageResponseTime.TotalMilliseconds.ToString("F0")ms</div>
        <div class="metric-label">Avg Response</div>
    </div>
    
    <div class="metric-card">
        <div class="metric-value">@LoaderService.Metrics.TotalRequests</div>
      <div class="metric-label">Total Requests</div>
  </div>
</div>

<h4>Status Codes</h4>
@foreach (var (code, count) in LoaderService.Metrics.StatusCodeDistribution)
{
    <div class="status-item">
  <span>HTTP @code:</span>
     <span>@count requests</span>
    </div>
}

@code {
    private System.Timers.Timer? _timer;

    protected override void OnInitialized()
    {
 _timer = new System.Timers.Timer(2000); // Refresh every 2s
        _timer.Elapsed += async (s, e) => await InvokeAsync(StateHasChanged);
        _timer.Start();
    }

    private string GetSuccessClass()
    {
        return LoaderService.Metrics.SuccessRate switch
        {
   >= 95 => "success",
   >= 80 => "warning",
            _ => "danger"
        };
}

    public void Dispose() => _timer?.Dispose();
}
```

### 4. Alertes automatiques

```csharp
public class MetricsMonitor : BackgroundService
{
    private readonly LoaderService _loaderService;
    private readonly INotificationService _notifications;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
  {
            var metrics = _loaderService.Metrics;
    
  if (metrics.FailureRate > 20 && metrics.TotalRequests >= 10)
{
  await _notifications.ShowWarning($"High failure rate: {metrics.FailureRate:F1}%");
         }
  
         if (metrics.AverageResponseTime.TotalSeconds > 5)
      {
  await _notifications.ShowWarning($"Slow response: {metrics.AverageResponseTime.TotalSeconds:F1}s");
   }
            
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken);
        }
    }
}
```

### 5. Export pour monitoring externe

```csharp
public async Task ExportMetricsAsync()
{
    var metrics = new
    {
        Timestamp = DateTime.UtcNow,
        TotalRequests = LoaderService.Metrics.TotalRequests,
      SuccessRate = LoaderService.Metrics.SuccessRate,
   FailureRate = LoaderService.Metrics.FailureRate,
        AvgResponseTime = LoaderService.Metrics.AverageResponseTime.TotalMilliseconds,
        StatusCodes = LoaderService.Metrics.StatusCodeDistribution
    };

    await analyticsClient.SendAsync("blazor-metrics", metrics);
}
```

## ?? CancellationToken Global

### Vue d'ensemble

Le `GlobalCancellationToken` permet d'annuler toutes les requêtes HTTP en cours simultanément.

### Utilisation automatique

Le handler HTTP combine automatiquement le `GlobalCancellationToken` avec le token de chaque requête :

```csharp
@inject IHttpClientFactory HttpClientFactory

var client = HttpClientFactory.CreateClient("BlazorFlexLoader");

// Le GlobalCancellationToken est automatiquement utilisé
var response = await client.GetAsync("/api/data");
```

### Annulation manuelle

```csharp
@inject LoaderService LoaderService

// Annuler toutes les requêtes en cours
LoaderService.CancelAllRequests();
```

### Utilisation dans du code personnalisé

```csharp
@inject LoaderService LoaderService
@inject HttpClient HttpClient

private async Task FetchDataAsync()
{
    try
    {
        LoaderService.Show();
 
        // Utiliser le GlobalCancellationToken
        var response = await HttpClient.GetAsync(
   "/api/data",
    LoaderService.GlobalCancellationToken);
     
        // Traiter la réponse
    }
    catch (OperationCanceledException)
    {
     // Requête annulée globalement
        Console.WriteLine("Request was cancelled");
    }
    finally
    {
      LoaderService.Close();
    }
}
```

### Cas d'usage

#### Navigation rapide

```csharp
@inject NavigationManager Navigation
@inject LoaderService LoaderService

protected override void OnInitialized()
{
    Navigation.LocationChanged += (s, e) =>
    {
        // Annuler toutes les requêtes lors de la navigation
        LoaderService.CancelAllRequests();
    };
}
```

#### Bouton "Annuler"

```razor
<button @onclick="CancelOperations">? Cancel All</button>

@code {
    private void CancelOperations()
    {
        LoaderService.CancelAllRequests();
    }
}
```

#### Timeout global

```csharp
protected override async Task OnInitializedAsync()
{
  // Annuler après 30 secondes
    _ = Task.Delay(TimeSpan.FromSeconds(30))
        .ContinueWith(_ => LoaderService.CancelAllRequests());

    await LoadDataAsync();
}
```

## ?? Réinitialisation des métriques

### Méthode 1 : Réinitialisation complète

```csharp
LoaderService.ResetMetrics();
```

### Méthode 2 : Réinitialisation périodique

```csharp
public class MetricsResetService : BackgroundService
{
    private readonly LoaderService _loaderService;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
      {
   await Task.Delay(TimeSpan.FromHours(24), stoppingToken);
    _loaderService.ResetMetrics();
        }
    }
}
```

## ?? Intégration avec Application Insights

```csharp
public class MetricsTracker : BackgroundService
{
    private readonly LoaderService _loaderService;
    private readonly TelemetryClient _telemetry;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
   {
            var metrics = _loaderService.Metrics;
   
 _telemetry.TrackMetric("HttpRequests.Total", metrics.TotalRequests);
       _telemetry.TrackMetric("HttpRequests.SuccessRate", metrics.SuccessRate);
      _telemetry.TrackMetric("HttpRequests.AvgResponseTime", 
        metrics.AverageResponseTime.TotalMilliseconds);
            
   await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }
}
```

## ?? Bonnes pratiques

### 1. Monitoring continu
```csharp
// Rafraîchir le dashboard toutes les 2-5 secondes
private System.Timers.Timer _metricsTimer = new(2000);
```

### 2. Alertes sur seuils
```csharp
if (metrics.FailureRate > 20) { /* Alert */ }
if (metrics.AverageResponseTime.TotalSeconds > 5) { /* Alert */ }
```

### 3. Export régulier
```csharp
// Exporter les métriques toutes les 5 minutes
await Task.Delay(TimeSpan.FromMinutes(5));
await ExportMetricsAsync();
```

### 4. Réinitialisation quotidienne
```csharp
// Réinitialiser à minuit
if (DateTime.Now.Hour == 0 && DateTime.Now.Minute == 0)
{
    LoaderService.ResetMetrics();
}
```

## ?? Voir aussi

- [Configuration avancée](./ADVANCED_CONFIGURATION.md)
- [Exemples de code](../Examples/MetricsAndCancellationExamples.cs)
- [Guide de mise à niveau](./UPGRADE_GUIDE_V1.6.md)
