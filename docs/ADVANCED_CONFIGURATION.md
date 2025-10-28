# Configuration avancée de Blazor.FlexLoader

## Options de l'intercepteur HTTP

### HttpInterceptorOptions

Classe de configuration pour personnaliser le comportement de l'intercepteur HTTP.

#### Propriétés disponibles

| Propriété | Type | Défaut | Description |
|-----------|------|--------|-------------|
| `MaxRetryAttempts` | `int` | `3` | Nombre maximum de tentatives de retry |
| `RetryDelay` | `TimeSpan` | `1s` | Délai de base entre les tentatives |
| `UseExponentialBackoff` | `bool` | `true` | Active l'augmentation exponentielle du délai |
| `RetryOnStatusCodes` | `HashSet<HttpStatusCode>` | `[500,502,503,504,408]` | Codes HTTP déclenchant un retry |
| `RetryOnTimeout` | `bool` | `true` | Retry en cas de timeout |
| `InterceptPredicate` | `Func<HttpRequestMessage, bool>?` | `null` | Filtre les requêtes à intercepter |
| `ShowLoaderPredicate` | `Func<HttpRequestMessage, bool>?` | `null` | Filtre l'affichage du loader |
| `OnRetry` | `Func<int, Exception?, TimeSpan, Task>?` | `null` | Callback avant chaque retry |
| `EnableDetailedLogging` | `bool` | `false` | Active le logging détaillé |

## Exemples de configuration

### 1. Configuration basique avec retry personnalisé

```csharp
builder.Services.AddBlazorFlexLoaderWithHttpInterceptor(
    client =>
    {
   client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    },
    options =>
    {
        options.MaxRetryAttempts = 5;
   options.RetryDelay = TimeSpan.FromSeconds(2);
    });
```

### 2. Exponential backoff

```csharp
builder.Services.AddBlazorFlexLoaderWithHttpInterceptor(
    configureOptions: options =>
    {
        options.UseExponentialBackoff = true;
     options.RetryDelay = TimeSpan.FromSeconds(1);
     // Tentatives : 1s, 2s, 4s, 8s, 16s...
    });
```

### 3. Intercepter uniquement certaines routes

```csharp
builder.Services.AddBlazorFlexLoaderWithHttpInterceptor(
    configureOptions: options =>
  {
      // N'intercepte que les appels API
        options.InterceptPredicate = request => 
            request.RequestUri?.AbsolutePath.StartsWith("/api/") ?? false;
    });
```

### 4. Afficher le loader uniquement pour certaines requêtes

```csharp
builder.Services.AddBlazorFlexLoaderWithHttpInterceptor(
    configureOptions: options =>
    {
        // Affiche le loader seulement pour POST, PUT, DELETE
      options.ShowLoaderPredicate = request => 
  request.Method != HttpMethod.Get;
    });
```

### 5. Personnaliser les codes de statut pour le retry

```csharp
builder.Services.AddBlazorFlexLoaderWithHttpInterceptor(
    configureOptions: options =>
    {
   options.RetryOnStatusCodes = new HashSet<HttpStatusCode>
      {
       HttpStatusCode.InternalServerError,  // 500
    HttpStatusCode.BadGateway,           // 502
            HttpStatusCode.TooManyRequests       // 429
        };
    });
```

### 6. Callback avant chaque retry

```csharp
@inject INotificationService NotificationService

builder.Services.AddBlazorFlexLoaderWithHttpInterceptor(
    configureOptions: options =>
    {
   options.OnRetry = async (attempt, exception, delay) =>
        {
    await NotificationService.ShowWarning(
  $"Tentative {attempt} après {delay.TotalSeconds}s...");
 };
    });
```

### 7. Configuration complète pour production

```csharp
builder.Services.AddBlazorFlexLoaderWithHttpInterceptor(
    client =>
  {
        client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
client.Timeout = TimeSpan.FromSeconds(30);
    },
 options =>
    {
        // Retry configuration
        options.MaxRetryAttempts = 3;
        options.UseExponentialBackoff = true;
        options.RetryDelay = TimeSpan.FromSeconds(1);
    
  // Status codes
        options.RetryOnStatusCodes = new HashSet<HttpStatusCode>
 {
   HttpStatusCode.InternalServerError,
            HttpStatusCode.BadGateway,
         HttpStatusCode.ServiceUnavailable,
     HttpStatusCode.GatewayTimeout
   };
      
        // Intercept only API calls
        options.InterceptPredicate = request => 
       request.RequestUri?.AbsolutePath.StartsWith("/api/") ?? false;
        
        // Show loader for mutations only
      options.ShowLoaderPredicate = request => 
   request.Method == HttpMethod.Post ||
            request.Method == HttpMethod.Put ||
            request.Method == HttpMethod.Delete;
    
        // Logging
      options.EnableDetailedLogging = builder.Environment.IsDevelopment();
        
    // Retry callback
     options.OnRetry = async (attempt, exception, delay) =>
  {
      Console.WriteLine($"Retry {attempt}/3 after {delay.TotalSeconds}s");
        };
});
```

### 8. Configuration pour développement

```csharp
#if DEBUG
builder.Services.AddBlazorFlexLoaderWithHttpInterceptor(
    configureOptions: options =>
    {
        options.MaxRetryAttempts = 1; // Pas de retry en dev
        options.EnableDetailedLogging = true;
   options.OnRetry = async (attempt, ex, delay) =>
        {
   Console.WriteLine($"[DEV] Retry: {ex?.Message}");
        };
    });
#else
builder.Services.AddBlazorFlexLoaderWithHttpInterceptor(
    configureOptions: options =>
    {
        options.MaxRetryAttempts = 3;
        options.UseExponentialBackoff = true;
    });
#endif
```

## Logging

Le handler utilise `ILogger<HttpCallInterceptorHandler>` pour tracer les requêtes :

- **LogInformation** : Début et fin de requête avec durée
- **LogWarning** : Tentatives de retry
- **LogError** : Échecs après épuisement des tentatives
- **LogDebug** : Détails techniques (si `EnableDetailedLogging = true`)

### Activer le logging dans Program.cs

```csharp
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Pour voir les logs de debug
#if DEBUG
builder.Logging.SetMinimumLevel(LogLevel.Debug);
#endif
```

## Comportement du retry

### Linear (UseExponentialBackoff = false)

```
Tentative 1: Immédiat
Tentative 2: +1s (RetryDelay)
Tentative 3: +1s (RetryDelay)
```

### Exponential Backoff (UseExponentialBackoff = true)

```
Tentative 1: Immédiat
Tentative 2: +1s  (RetryDelay × 2^0)
Tentative 3: +2s  (RetryDelay × 2^1)
Tentative 4: +4s  (RetryDelay × 2^2)
Tentative 5: +8s  (RetryDelay × 2^3)
```

## Cas d'usage recommandés

| Scénario | Configuration recommandée |
|----------|---------------------------|
| **API instable** | `MaxRetryAttempts = 5`, `UseExponentialBackoff = true` |
| **Haute disponibilité** | `RetryOnStatusCodes = [500,502,503,504]` |
| **Mutations critiques** | `ShowLoaderPredicate` pour POST/PUT/DELETE uniquement |
| **SPA avec navigation** | `InterceptPredicate` pour filtrer les routes non-API |
| **Debugging** | `EnableDetailedLogging = true`, `OnRetry` avec logs |

## Migration depuis la version précédente

### Avant (v1.4.0)

```csharp
builder.Services.AddBlazorFlexLoaderWithHttpInterceptor(client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});
```

### Après (v1.5.0+)

```csharp
// Fonctionne toujours ! (rétrocompatible)
builder.Services.AddBlazorFlexLoaderWithHttpInterceptor(client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});

// Ou avec les nouvelles options
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

## Support

Pour toute question ou problème, ouvrez une issue sur [GitHub](https://github.com/daniwxcode/Blazor.FlexLoader/issues).
