# Guide de mise � niveau v1.6.0

## ?? Nouvelles fonctionnalit�s

### 1. ?? M�triques en temps r�el

Surveillance compl�te des performances des requ�tes HTTP.

```csharp
@inject LoaderService LoaderService

// Acc�der aux m�triques
var metrics = LoaderService.Metrics;
Console.WriteLine($"Success Rate: {metrics.SuccessRate}%");
Console.WriteLine($"Avg Response: {metrics.AverageResponseTime.TotalMilliseconds}ms");

// R�sum� format�
Console.WriteLine(LoaderService.GetMetricsSummary());
```

**M�triques disponibles :**
- Total/Succ�s/�checs des requ�tes
- Taux de retry
- Temps de r�ponse (min/max/moyen)
- Distribution des codes HTTP
- Derni�re requ�te

### 2. ?? CancellationToken global

Annulation de toutes les requ�tes en cours simultan�ment.

```csharp
@inject LoaderService LoaderService

// Annuler toutes les requ�tes
LoaderService.CancelAllRequests();

// Utiliser le token global
var response = await httpClient.GetAsync(
    "/api/data",
    LoaderService.GlobalCancellationToken);
```

## ?? Migration depuis v1.5.0

### Aucune modification requise ! ?

La v1.6.0 est **100% r�trocompatible** avec la v1.5.0.

Votre code existant continue de fonctionner sans changement :

```csharp
// Code v1.5.0 - Fonctionne toujours !
builder.Services.AddBlazorFlexLoaderWithHttpInterceptor(
    client =>
    {
client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    },
    options =>
{
        options.MaxRetryAttempts = 5;
   options.UseExponentialBackoff = true;
    });
```

### Nouvelles fonctionnalit�s disponibles imm�diatement

Les m�triques sont **automatiquement** activ�es et collect�es :

```csharp
// Pas de configuration n�cessaire !
// Les m�triques sont d�j� disponibles
var metrics = LoaderService.Metrics;
```

## ?? Utilisation des nouvelles fonctionnalit�s

### Dashboard de monitoring

```razor
@page "/admin/dashboard"
@inject LoaderService LoaderService

<h3>?? System Dashboard</h3>

<div class="metrics">
    <div class="card">
        <h4>@LoaderService.Metrics.TotalRequests</h4>
        <p>Total Requests</p>
    </div>
    
    <div class="card success">
        <h4>@LoaderService.Metrics.SuccessRate.ToString("F1")%</h4>
        <p>Success Rate</p>
    </div>
    
    <div class="card">
   <h4>@LoaderService.Metrics.AverageResponseTime.TotalMilliseconds.ToString("F0")ms</h4>
     <p>Avg Response</p>
    </div>
</div>

<button @onclick="ResetMetrics">Reset Metrics</button>
<button @onclick="CancelAll">Cancel All Requests</button>

@code {
 private void ResetMetrics() => LoaderService.ResetMetrics();
    private void CancelAll() => LoaderService.CancelAllRequests();
}
```

### Annulation lors de la navigation

```csharp
@inject NavigationManager Navigation
@inject LoaderService LoaderService

protected override void OnInitialized()
{
    Navigation.LocationChanged += (s, e) =>
    {
     // Annuler automatiquement les requ�tes lors de la navigation
   LoaderService.CancelAllRequests();
    };
}
```

### Export des m�triques

```csharp
public async Task ExportToMonitoringAsync()
{
var metrics = new
    {
  timestamp = DateTime.UtcNow,
   total_requests = LoaderService.Metrics.TotalRequests,
        success_rate = LoaderService.Metrics.SuccessRate,
     avg_response_ms = LoaderService.Metrics.AverageResponseTime.TotalMilliseconds
    };

    await monitoringService.SendAsync("blazor-metrics", metrics);
}
```

## ?? Comparaison des versions

| Fonctionnalit� | v1.5.0 | v1.6.0 |
|----------------|--------|--------|
| Retry avec exponential backoff | ? | ? |
| Logging ILogger | ? | ? |
| Filtrage conditionnel | ? | ? |
| **M�triques en temps r�el** | ? | ? NEW |
| **CancellationToken global** | ? | ? NEW |
| **Distribution codes HTTP** | ? | ? NEW |
| **Temps de r�ponse stats** | ? | ? NEW |
| **Taux de succ�s/�chec** | ? | ? NEW |

## ?? Cas d'usage recommand�s

### 1. Monitoring en production

```csharp
// Cr�er un service de monitoring
public class ProductionMonitor : BackgroundService
{
  private readonly LoaderService _loader;
 private readonly ILogger _logger;

    protected override async Task ExecuteAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
         var m = _loader.Metrics;
    
    if (m.FailureRate > 10)
       _logger.LogWarning("High failure rate: {Rate}%", m.FailureRate);
   
   if (m.AverageResponseTime.TotalSeconds > 3)
  _logger.LogWarning("Slow responses: {Time}s", m.AverageResponseTime.TotalSeconds);
 
        await Task.Delay(TimeSpan.FromMinutes(1), ct);
        }
    }
}
```

### 2. Dashboard d'administration

```razor
@page "/admin"
@inject LoaderService LoaderService

<h3>System Health</h3>

@if (LoaderService.Metrics.FailureRate > 20)
{
    <div class="alert alert-danger">
   ?? High failure rate detected!
 </div>
}

<pre>@LoaderService.GetMetricsSummary()</pre>
```

### 3. Annulation rapide

```razor
<button @onclick="HandleNavigation">Navigate Away</button>

@code {
    private void HandleNavigation()
    {
  // Annuler les requ�tes avant de naviguer
        LoaderService.CancelAllRequests();
        NavigationManager.NavigateTo("/other-page");
    }
}
```

## ?? Documentation compl�te

- [Guide des m�triques et CancellationToken](./METRICS_AND_CANCELLATION.md)
- [Exemples de code](../Examples/MetricsAndCancellationExamples.cs)
- [Configuration avanc�e](./ADVANCED_CONFIGURATION.md)

## ? FAQ

### Les m�triques affectent-elles les performances ?

Non, l'impact est minimal. Les m�triques utilisent des structures thread-safe optimis�es et ne conservent que les 100 derniers temps de r�ponse.

### Puis-je d�sactiver les m�triques ?

Les m�triques sont toujours collect�es, mais vous n'�tes pas oblig� de les utiliser. Il n'y a pas d'overhead significatif.

### Le GlobalCancellationToken annule-t-il les requ�tes automatiquement ?

Non, vous devez appeler `CancelAllRequests()` manuellement. Le token est automatiquement utilis� par le handler HTTP.

### Que se passe-t-il si j'appelle ResetMetrics() ?

Toutes les statistiques sont remises � z�ro. Les requ�tes en cours ne sont pas affect�es.

### Les m�triques sont-elles partag�es entre les utilisateurs ?

Oui, le `LoaderService` est `Scoped`, donc les m�triques sont partag�es au sein d'une m�me session utilisateur (circuit Blazor).

## ?? Prochaines �tapes

1. ? Mettre � jour vers v1.6.0
2. ? Ajouter un dashboard de m�triques (optionnel)
3. ? Impl�menter l'annulation sur navigation (recommand�)
4. ? Configurer des alertes de monitoring (production)
5. ? Exporter les m�triques vers votre syst�me de monitoring (optionnel)

Bonne mise � niveau ! ??
