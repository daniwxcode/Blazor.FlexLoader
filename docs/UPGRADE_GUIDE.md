# Blazor.FlexLoader - Guide de mise � niveau v1.5.0

## ?? Nouvelles fonctionnalit�s

### 1. Configuration avanc�e du retry avec exponential backoff

```csharp
builder.Services.AddBlazorFlexLoaderWithHttpInterceptor(
    configureOptions: options =>
    {
        options.MaxRetryAttempts = 5;
        options.UseExponentialBackoff = true;  // 1s, 2s, 4s, 8s, 16s...
      options.RetryDelay = TimeSpan.FromSeconds(1);
    });
```

**B�n�fices :**
- ?? R�duction de la charge sur le serveur
- ?? Meilleure r�silience face aux erreurs temporaires
- ?? Strat�gie intelligente d'attente progressive

### 2. Logging int�gr� avec ILogger

```csharp
// Automatiquement activ� si ILogger est disponible
builder.Logging.AddConsole();

builder.Services.AddBlazorFlexLoaderWithHttpInterceptor(
    configureOptions: options =>
    {
        options.EnableDetailedLogging = builder.Environment.IsDevelopment();
  });
```

**Logs g�n�r�s :**
```
[Information] Starting request to https://api.example.com/data (Method: GET)
[Warning] Request to https://api.example.com/data returned InternalServerError, attempt 1/3
[Debug] Waiting 1000ms before retry attempt 2
[Information] Request to https://api.example.com/data completed in 2345ms with status OK
```

### 3. Filtrage conditionnel des requ�tes

#### Intercepter uniquement les routes API

```csharp
options.InterceptPredicate = request => 
    request.RequestUri?.AbsolutePath.StartsWith("/api/") ?? false;
```

#### Loader uniquement pour les mutations

```csharp
options.ShowLoaderPredicate = request => 
    request.Method == HttpMethod.Post ||
    request.Method == HttpMethod.Put ||
    request.Method == HttpMethod.Delete;
```

**B�n�fices :**
- ? Meilleure performance (moins de requ�tes intercept�es)
- ?? UX cibl�e (loader seulement quand n�cessaire)
- ?? Flexibilit� totale

### 4. Callbacks personnalisables

```csharp
options.OnRetry = async (attempt, exception, delay) =>
{
 await NotificationService.ShowWarning($"Nouvelle tentative {attempt}/3...");
    await AnalyticsService.TrackRetry(exception);
};
```

**Cas d'usage :**
- ?? Notifications utilisateur
- ?? Tracking analytics
- ?? Debugging avanc�
- ?? Logging personnalis�

### 5. Codes de statut HTTP personnalisables

```csharp
options.RetryOnStatusCodes = new HashSet<HttpStatusCode>
{
    HttpStatusCode.InternalServerError,  // 500
    HttpStatusCode.BadGateway,           // 502
    HttpStatusCode.ServiceUnavailable,   // 503
  HttpStatusCode.GatewayTimeout,       // 504
    HttpStatusCode.TooManyRequests   // 429
};
```

## ?? Guide de migration

### Version actuelle ? v1.5.0

**Aucune modification requise !** 100% r�trocompatible.

#### Avant (v1.4.0)
```csharp
builder.Services.AddBlazorFlexLoaderWithHttpInterceptor(client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});
```

#### Apr�s (v1.5.0) - Fonctionne toujours !
```csharp
// M�me syntaxe, comportement identique
builder.Services.AddBlazorFlexLoaderWithHttpInterceptor(client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});
```

#### Apr�s (v1.5.0) - Avec nouvelles options (optionnel)
```csharp
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

## ?? Sc�narios recommand�s

### API instable / r�seau peu fiable
```csharp
options.MaxRetryAttempts = 5;
options.UseExponentialBackoff = true;
options.RetryDelay = TimeSpan.FromSeconds(2);
```

### Application avec navigation SPA
```csharp
options.InterceptPredicate = request => 
    request.RequestUri?.AbsolutePath.StartsWith("/api/") ?? false;
options.ShowLoaderPredicate = request => 
    request.Method != HttpMethod.Get;
```

### Environnement de d�veloppement
```csharp
options.EnableDetailedLogging = true;
options.MaxRetryAttempts = 1;
options.OnRetry = async (attempt, ex, delay) =>
{
    Console.WriteLine($"[DEV] Retry: {ex?.Message}");
};
```

### Production haute disponibilit�
```csharp
options.MaxRetryAttempts = 3;
options.UseExponentialBackoff = true;
options.RetryOnStatusCodes = new[] { 500, 502, 503, 504 };
options.EnableDetailedLogging = false;
```

## ?? Comparaison des versions

| Fonctionnalit� | v1.4.0 | v1.5.0 |
|----------------|--------|--------|
| Retry basique | ? 3 tentatives fixes | ? Configurable |
| Exponential backoff | ? | ? |
| Logging | ? | ? ILogger int�gr� |
| Filtrage conditionnel | ? | ? Predicates |
| Callbacks | ? | ? OnRetry |
| Codes HTTP personnalisables | ? | ? HashSet configurable |
| Documentation | ? Basique | ? Compl�te avec exemples |

## ?? Ressources

- ?? [Documentation compl�te](./ADVANCED_CONFIGURATION.md)
- ?? [CHANGELOG](../CHANGELOG.md)
- ?? [Exemples de code](../Examples/ConfigurationExamples.cs)
- ?? [Issues GitHub](https://github.com/daniwxcode/Blazor.FlexLoader/issues)

## ? FAQ

### Est-ce que je dois changer mon code existant ?
Non ! La v1.5.0 est 100% r�trocompatible. Votre code actuel continuera de fonctionner.

### Comment activer le logging ?
Le logging est automatique si `ILogger<HttpCallInterceptorHandler>` est disponible. Ajoutez simplement :
```csharp
builder.Logging.AddConsole();
```

### Quelle est la diff�rence entre InterceptPredicate et ShowLoaderPredicate ?
- `InterceptPredicate` : Filtre les requ�tes � intercepter (retry, logging, etc.)
- `ShowLoaderPredicate` : Filtre l'affichage du loader uniquement

Vous pouvez intercepter toutes les requ�tes mais afficher le loader seulement pour certaines.

### L'exponential backoff est-il activ� par d�faut ?
Oui, `UseExponentialBackoff = true` par d�faut. Pour un d�lai constant, d�finissez-le � `false`.

### Comment d�sactiver le retry ?
```csharp
options.MaxRetryAttempts = 1;
```

### Puis-je avoir des logs diff�rents en dev et en prod ?
Oui :
```csharp
options.EnableDetailedLogging = builder.Environment.IsDevelopment();
```

## ?? Prochaines �tapes

1. ? Tester les nouvelles options dans votre environnement de dev
2. ? Configurer le logging appropri�
3. ? D�finir les predicates selon vos besoins
4. ? Ajuster les param�tres de retry pour votre API
5. ? D�ployer en production

Bon upgrade ! ??
