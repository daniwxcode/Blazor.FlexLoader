# Documentation Blazor.FlexLoader

Bienvenue dans la documentation complète de **Blazor.FlexLoader** !

## ?? Table des matières

### ?? Démarrage rapide
- [README principal](../README.md) - Vue d'ensemble et installation
- [Guide de mise à niveau v1.5.0](./UPGRADE_GUIDE.md) - Nouvelles fonctionnalités et migration

### ?? Configuration
- [Configuration avancée](./ADVANCED_CONFIGURATION.md) - Toutes les options détaillées
- [Exemples de configuration](../Examples/ConfigurationExamples.cs) - Code prêt à l'emploi

### ?? Guides
- [CHANGELOG](../CHANGELOG.md) - Historique des versions
- [LoaderService Documentation](./LoaderService-Documentation.md) - API complète du service

## ?? Navigation rapide

### Par fonctionnalité

#### Interception HTTP
- [Configuration de base](./ADVANCED_CONFIGURATION.md#1-configuration-basique-avec-retry-personnalisé)
- [Exponential backoff](./ADVANCED_CONFIGURATION.md#2-exponential-backoff)
- [Retry personnalisé](./ADVANCED_CONFIGURATION.md#5-personnaliser-les-codes-de-statut-pour-le-retry)

#### Filtrage
- [Intercepter certaines routes](./ADVANCED_CONFIGURATION.md#3-intercepter-uniquement-certaines-routes)
- [Loader conditionnel](./ADVANCED_CONFIGURATION.md#4-afficher-le-loader-uniquement-pour-certaines-requêtes)

#### Logging
- [Configuration du logging](./ADVANCED_CONFIGURATION.md#activer-le-logging-dans-programcs)
- [Logging détaillé](./ADVANCED_CONFIGURATION.md#7-configuration-pour-développement)

#### Callbacks
- [Notifications de retry](./ADVANCED_CONFIGURATION.md#6-callback-avant-chaque-retry)
- [Monitoring personnalisé](./ADVANCED_CONFIGURATION.md#7-configuration-complète-pour-production)

### Par scénario

| Scénario | Documentation |
|----------|---------------|
| **Première utilisation** | [README principal](../README.md) |
| **Migration depuis v1.4.0** | [Guide de mise à niveau](./UPGRADE_GUIDE.md) |
| **API instable** | [Configuration avancée - Cas d'usage](./ADVANCED_CONFIGURATION.md#cas-dusage-recommandés) |
| **Application SPA** | [Configuration SPA](./ADVANCED_CONFIGURATION.md#9-configuration-pour-une-spa) |
| **Environnement de développement** | [Configuration développement](./ADVANCED_CONFIGURATION.md#7-configuration-pour-développement) |
| **Production** | [Configuration production](./ADVANCED_CONFIGURATION.md#7-configuration-complète-pour-production) |

## ?? Nouveautés v1.5.0

### Configuration avancée du retry
```csharp
options.MaxRetryAttempts = 5;
options.UseExponentialBackoff = true;
options.RetryDelay = TimeSpan.FromSeconds(1);
```

### Logging intégré
```csharp
options.EnableDetailedLogging = true;
```

### Filtrage conditionnel
```csharp
options.InterceptPredicate = request => 
    request.RequestUri?.AbsolutePath.StartsWith("/api/") ?? false;
```

### Callbacks personnalisables
```csharp
options.OnRetry = async (attempt, exception, delay) =>
{
    await NotificationService.ShowWarning($"Tentative {attempt}...");
};
```

[Voir toutes les nouveautés ?](./UPGRADE_GUIDE.md#-nouvelles-fonctionnalités)

## ?? Référence API

### Classes principales

#### `LoaderService`
Service principal de gestion du loader.
- [Documentation complète](./LoaderService-Documentation.md)

#### `HttpCallInterceptorHandler`
Handler d'interception des requêtes HTTP.
- [Configuration](./ADVANCED_CONFIGURATION.md)
- [Exemples](../Examples/ConfigurationExamples.cs)

#### `HttpInterceptorOptions`
Options de configuration pour l'intercepteur.
- [Propriétés disponibles](./ADVANCED_CONFIGURATION.md#propriétés-disponibles)

### Extensions

#### `AddBlazorFlexLoader()`
Configuration basique sans interception HTTP.
```csharp
services.AddBlazorFlexLoader();
```

#### `AddBlazorFlexLoaderWithHttpInterceptor()`
Configuration avec interception HTTP.
```csharp
services.AddBlazorFlexLoaderWithHttpInterceptor(
    configureClient: client => { },
    configureOptions: options => { });
```

## ?? Exemples de code

Tous les exemples sont disponibles dans :
- [ConfigurationExamples.cs](../Examples/ConfigurationExamples.cs)
- [ADVANCED_CONFIGURATION.md](./ADVANCED_CONFIGURATION.md)

### Exemples rapides

#### Configuration minimale
```csharp
services.AddBlazorFlexLoader();
```

#### Avec interception HTTP
```csharp
services.AddBlazorFlexLoaderWithHttpInterceptor(client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
});
```

#### Configuration complète
```csharp
services.AddBlazorFlexLoaderWithHttpInterceptor(
    client =>
    {
        client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
        client.Timeout = TimeSpan.FromSeconds(30);
    },
    options =>
 {
    options.MaxRetryAttempts = 5;
        options.UseExponentialBackoff = true;
        options.InterceptPredicate = request => 
            request.RequestUri?.AbsolutePath.StartsWith("/api/") ?? false;
  options.OnRetry = async (attempt, ex, delay) =>
        {
    Console.WriteLine($"Retry {attempt}");
        };
    });
```

## ?? Recherche par mot-clé

- **Retry** ? [Exponential backoff](./ADVANCED_CONFIGURATION.md#2-exponential-backoff), [Configuration](./ADVANCED_CONFIGURATION.md#5-personnaliser-les-codes-de-statut-pour-le-retry)
- **Logging** ? [Configuration](./ADVANCED_CONFIGURATION.md#logging), [Développement](./ADVANCED_CONFIGURATION.md#7-configuration-pour-développement)
- **Filtrage** ? [InterceptPredicate](./ADVANCED_CONFIGURATION.md#3-intercepter-uniquement-certaines-routes), [ShowLoaderPredicate](./ADVANCED_CONFIGURATION.md#4-afficher-le-loader-uniquement-pour-certaines-requêtes)
- **Callbacks** ? [OnRetry](./ADVANCED_CONFIGURATION.md#6-callback-avant-chaque-retry)
- **Performance** ? [Filtrage](./ADVANCED_CONFIGURATION.md#cas-dusage-recommandés), [Logging conditionnel](./ADVANCED_CONFIGURATION.md#7-configuration-pour-développement)
- **Production** ? [Configuration complète](./ADVANCED_CONFIGURATION.md#7-configuration-complète-pour-production)
- **Développement** ? [Configuration dev](./ADVANCED_CONFIGURATION.md#8-configuration-pour-développement)

## ?? Contribution

Pour contribuer à la documentation :
1. Créez une issue sur [GitHub](https://github.com/daniwxcode/Blazor.FlexLoader/issues)
2. Proposez une Pull Request avec vos améliorations

## ?? Support

- ?? [Issues GitHub](https://github.com/daniwxcode/Blazor.FlexLoader/issues)
- ?? Contact : voir le profil GitHub [@daniwxcode](https://github.com/daniwxcode)

## ?? Licence

MIT - Voir [LICENSE](../LICENSE) pour plus de détails.

---

**Version de la documentation :** 1.5.0  
**Dernière mise à jour :** Janvier 2025
