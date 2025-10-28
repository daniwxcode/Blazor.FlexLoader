# Guide de mise à niveau v1.6.0

## ?? Nouvelles fonctionnalités

### 1. ?? Métriques en temps réel

Surveillance complète des performances des requêtes HTTP.

```csharp
@inject LoaderService LoaderService

// Accéder aux métriques
var metrics = LoaderService.Metrics;
Console.WriteLine($"Success Rate: {metrics.SuccessRate}%");
Console.WriteLine($"Avg Response: {metrics.AverageResponseTime.TotalMilliseconds}ms");

// Résumé formaté
Console.WriteLine(LoaderService.GetMetricsSummary());
```

**Métriques disponibles :**
- Total/Succès/Échecs des requêtes
- Taux de retry
- Temps de réponse (min/max/moyen)
- Distribution des codes HTTP
- Dernière requête

### 2. ?? CancellationToken global

Annulation de toutes les requêtes en cours simultanément.

```csharp
@inject LoaderService LoaderService

// Annuler toutes les requêtes
LoaderService.CancelAllRequests();

// Utiliser le token global
var response = await httpClient.GetAsync(
    "/api/data",
    LoaderService.GlobalCancellationToken);
```

## ?? Migration depuis v1.5.0

### Aucune modification requise ! ?

La v1.6.0 est **100% rétrocompatible** avec la v1.5.0.

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

### Nouvelles fonctionnalités disponibles immédiatement

Les métriques sont **automatiquement** activées et collectées :

```csharp
// Pas de configuration nécessaire !
// Les métriques sont déjà disponibles
var metrics = LoaderService.Metrics;
```

## ?? Utilisation des nouvelles fonctionnalités

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
     // Annuler automatiquement les requêtes lors de la navigation
   LoaderService.CancelAllRequests();
    };
}
```

### Export des métriques

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

| Fonctionnalité | v1.5.0 | v1.6.0 |
|----------------|--------|--------|
| Retry avec exponential backoff | ? | ? |
| Logging ILogger | ? | ? |
| Filtrage conditionnel | ? | ? |
| **Métriques en temps réel** | ? | ? NEW |
| **CancellationToken global** | ? | ? NEW |
| **Distribution codes HTTP** | ? | ? NEW |
| **Temps de réponse stats** | ? | ? NEW |
| **Taux de succès/échec** | ? | ? NEW |

## ?? Cas d'usage recommandés

### 1. Monitoring en production

```csharp
// Créer un service de monitoring
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
  // Annuler les requêtes avant de naviguer
        LoaderService.CancelAllRequests();
        NavigationManager.NavigateTo("/other-page");
    }
}
```

## ?? Documentation complète

- [Guide des métriques et CancellationToken](./METRICS_AND_CANCELLATION.md)
- [Exemples de code](../Examples/MetricsAndCancellationExamples.cs)
- [Configuration avancée](./ADVANCED_CONFIGURATION.md)

## ? FAQ

### Les métriques affectent-elles les performances ?

Non, l'impact est minimal. Les métriques utilisent des structures thread-safe optimisées et ne conservent que les 100 derniers temps de réponse.

### Puis-je désactiver les métriques ?

Les métriques sont toujours collectées, mais vous n'êtes pas obligé de les utiliser. Il n'y a pas d'overhead significatif.

### Le GlobalCancellationToken annule-t-il les requêtes automatiquement ?

Non, vous devez appeler `CancelAllRequests()` manuellement. Le token est automatiquement utilisé par le handler HTTP.

### Que se passe-t-il si j'appelle ResetMetrics() ?

Toutes les statistiques sont remises à zéro. Les requêtes en cours ne sont pas affectées.

### Les métriques sont-elles partagées entre les utilisateurs ?

Oui, le `LoaderService` est `Scoped`, donc les métriques sont partagées au sein d'une même session utilisateur (circuit Blazor).

## ?? Prochaines étapes

1. ? Mettre à jour vers v1.6.0
2. ? Ajouter un dashboard de métriques (optionnel)
3. ? Implémenter l'annulation sur navigation (recommandé)
4. ? Configurer des alertes de monitoring (production)
5. ? Exporter les métriques vers votre système de monitoring (optionnel)

Bonne mise à niveau ! ??
