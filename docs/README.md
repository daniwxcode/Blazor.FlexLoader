# Documentation Blazor.FlexLoader

Bienvenue dans la documentation compl�te de **Blazor.FlexLoader** !

## ?? Table des mati�res

### ?? D�marrage rapide
- [README principal](../README.md) - Vue d'ensemble et installation
- [Guide de mise � niveau v1.5.0](./UPGRADE_GUIDE.md) - Nouvelles fonctionnalit�s et migration

### ?? Configuration
- [Configuration avanc�e](./ADVANCED_CONFIGURATION.md) - Toutes les options d�taill�es
- [Exemples de configuration](../Examples/ConfigurationExamples.cs) - Code pr�t � l'emploi

### ?? Guides
- [CHANGELOG](../CHANGELOG.md) - Historique des versions
- [LoaderService Documentation](./LoaderService-Documentation.md) - API compl�te du service

## ?? Navigation rapide

### Par fonctionnalit�

#### Interception HTTP
- [Configuration de base](./ADVANCED_CONFIGURATION.md#1-configuration-basique-avec-retry-personnalis�)
- [Exponential backoff](./ADVANCED_CONFIGURATION.md#2-exponential-backoff)
- [Retry personnalis�](./ADVANCED_CONFIGURATION.md#5-personnaliser-les-codes-de-statut-pour-le-retry)

#### Filtrage
- [Intercepter certaines routes](./ADVANCED_CONFIGURATION.md#3-intercepter-uniquement-certaines-routes)
- [Loader conditionnel](./ADVANCED_CONFIGURATION.md#4-afficher-le-loader-uniquement-pour-certaines-requ�tes)

#### Logging
- [Configuration du logging](./ADVANCED_CONFIGURATION.md#activer-le-logging-dans-programcs)
- [Logging d�taill�](./ADVANCED_CONFIGURATION.md#7-configuration-pour-d�veloppement)

#### Callbacks
- [Notifications de retry](./ADVANCED_CONFIGURATION.md#6-callback-avant-chaque-retry)
- [Monitoring personnalis�](./ADVANCED_CONFIGURATION.md#7-configuration-compl�te-pour-production)

### Par sc�nario

| Sc�nario | Documentation |
|----------|---------------|
| **Premi�re utilisation** | [README principal](../README.md) |
| **Migration depuis v1.4.0** | [Guide de mise � niveau](./UPGRADE_GUIDE.md) |
| **API instable** | [Configuration avanc�e - Cas d'usage](./ADVANCED_CONFIGURATION.md#cas-dusage-recommand�s) |
| **Application SPA** | [Configuration SPA](./ADVANCED_CONFIGURATION.md#9-configuration-pour-une-spa) |
| **Environnement de d�veloppement** | [Configuration d�veloppement](./ADVANCED_CONFIGURATION.md#7-configuration-pour-d�veloppement) |
| **Production** | [Configuration production](./ADVANCED_CONFIGURATION.md#7-configuration-compl�te-pour-production) |

## ?? Nouveaut�s v1.5.0

### Configuration avanc�e du retry
```csharp
options.MaxRetryAttempts = 5;
options.UseExponentialBackoff = true;
options.RetryDelay = TimeSpan.FromSeconds(1);
```

### Logging int�gr�
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

[Voir toutes les nouveaut�s ?](./UPGRADE_GUIDE.md#-nouvelles-fonctionnalit�s)

## ?? R�f�rence API

### Classes principales

#### `LoaderService`
Service principal de gestion du loader.
- [Documentation compl�te](./LoaderService-Documentation.md)

#### `HttpCallInterceptorHandler`
Handler d'interception des requ�tes HTTP.
- [Configuration](./ADVANCED_CONFIGURATION.md)
- [Exemples](../Examples/ConfigurationExamples.cs)

#### `HttpInterceptorOptions`
Options de configuration pour l'intercepteur.
- [Propri�t�s disponibles](./ADVANCED_CONFIGURATION.md#propri�t�s-disponibles)

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

#### Configuration compl�te
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

## ?? Recherche par mot-cl�

- **Retry** ? [Exponential backoff](./ADVANCED_CONFIGURATION.md#2-exponential-backoff), [Configuration](./ADVANCED_CONFIGURATION.md#5-personnaliser-les-codes-de-statut-pour-le-retry)
- **Logging** ? [Configuration](./ADVANCED_CONFIGURATION.md#logging), [D�veloppement](./ADVANCED_CONFIGURATION.md#7-configuration-pour-d�veloppement)
- **Filtrage** ? [InterceptPredicate](./ADVANCED_CONFIGURATION.md#3-intercepter-uniquement-certaines-routes), [ShowLoaderPredicate](./ADVANCED_CONFIGURATION.md#4-afficher-le-loader-uniquement-pour-certaines-requ�tes)
- **Callbacks** ? [OnRetry](./ADVANCED_CONFIGURATION.md#6-callback-avant-chaque-retry)
- **Performance** ? [Filtrage](./ADVANCED_CONFIGURATION.md#cas-dusage-recommand�s), [Logging conditionnel](./ADVANCED_CONFIGURATION.md#7-configuration-pour-d�veloppement)
- **Production** ? [Configuration compl�te](./ADVANCED_CONFIGURATION.md#7-configuration-compl�te-pour-production)
- **D�veloppement** ? [Configuration dev](./ADVANCED_CONFIGURATION.md#8-configuration-pour-d�veloppement)

## ?? Contribution

Pour contribuer � la documentation :
1. Cr�ez une issue sur [GitHub](https://github.com/daniwxcode/Blazor.FlexLoader/issues)
2. Proposez une Pull Request avec vos am�liorations

## ?? Support

- ?? [Issues GitHub](https://github.com/daniwxcode/Blazor.FlexLoader/issues)
- ?? Contact : voir le profil GitHub [@daniwxcode](https://github.com/daniwxcode)

## ?? Licence

MIT - Voir [LICENSE](../LICENSE) pour plus de d�tails.

---

**Version de la documentation :** 1.5.0  
**Derni�re mise � jour :** Janvier 2025
