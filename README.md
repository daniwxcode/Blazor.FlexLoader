# Blazor.FlexLoader

[![CI](https://github.com/daniwxcode/Blazor.FlexLoader/actions/workflows/ci.yml/badge.svg)](https://github.com/daniwxcode/Blazor.FlexLoader/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Blazor.FlexLoader.svg)](https://www.nuget.org/packages/Blazor.FlexLoader/)
[![Downloads](https://img.shields.io/nuget/dt/Blazor.FlexLoader.svg)](https://www.nuget.org/packages/Blazor.FlexLoader/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

<div align="center">
  <img src="./assets/icon-github.svg" alt="Blazor.FlexLoader" width="128" height="128" />
  <h3>Un composant Blazor flexible pour les indicateurs de chargement</h3>
</div>

[Francais](#francais) | [English](#english)

---

## Francais

Un composant Blazor flexible et reutilisable pour afficher des indicateurs de chargement avec support pour des images personnalisees, du texte et du contenu custom. **Inclut un SVG anime par defaut !**

### Fonctionnalites

- **SVG anime par defaut** - Aucune configuration requise !
- **Interception automatique HTTP** - Affiche le loader pendant les requetes HTTP
- **Retry automatique** - Reessaie les requetes echouees jusqu'a 3 fois
- **Exponential backoff** - Délai intelligent entre les tentatives
- **Configuration avancée** - Options personnalisables pour le retry et l'interception
- **Logging intégré** - Traçabilité complète avec ILogger
- **Filtrage conditionnel** - Intercepte uniquement certaines routes ou méthodes
- Indicateur de chargement global pour les applications Blazor
- Support d'images personnalisees (GIF, SVG, PNG, etc.)
- Texte de chargement personnalisable
- Contenu custom avec RenderFragment
- Positionnement absolu ou fixe
- Couleurs et styles personnalisables
- Fermeture automatique configurable
- Fermeture au clic sur l'overlay
- Compatible .NET 9
- CI/CD automatise avec GitHub Actions

### Installation

```bash
dotnet add package Blazor.FlexLoader
```

### Configuration

#### Option 1: Configuration basique

##### 1. Enregistrer le service dans `Program.cs`

```csharp
using Blazor.FlexLoader.Extensions;

builder.Services.AddBlazorFlexLoader();
```

##### 2. Ajouter les imports dans `_Imports.razor`

```razor
@using Blazor.FlexLoader.Components
@using Blazor.FlexLoader.Services
```

##### 3. Ajouter le composant dans votre layout

```razor
<FlexLoader />
```

**C'est tout ! Le loader affichera automatiquement un SVG anime professionnel.**

#### Option 2: Configuration avec interception HTTP (Recommande)

##### 1. Enregistrer les services avec interception HTTP dans `Program.cs`

```csharp
using Blazor.FlexLoader.Extensions;

// Configure le loader avec interception automatique des requetes HTTP
builder.Services.AddBlazorFlexLoaderWithHttpInterceptor(client =>
{
 client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    // Autres configurations du HttpClient...
});
```

##### 2. Ajouter les imports dans `_Imports.razor`

```razor
@using Blazor.FlexLoader.Components
@using Blazor.FlexLoader.Services
```

##### 3. Ajouter le composant dans votre layout

```razor
<FlexLoader />
```

**Avantages:**
- ? Le loader s'affiche automatiquement pendant **toutes** les requetes HTTP
- ? Retry automatique (3 tentatives) en cas d'erreur HTTP 500 ou `HttpRequestException`
- ? Pas besoin d'appeler manuellement `Show()` et `Close()`
- ? Gestion centralisee des erreurs reseau

#### Option 3: Configuration avancée avec options personnalisées ??

```csharp
using Blazor.FlexLoader.Extensions;

builder.Services.AddBlazorFlexLoaderWithHttpInterceptor(
    client =>
    {
        client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
        client.Timeout = TimeSpan.FromSeconds(30);
    },
    options =>
    {
        // Retry avec exponential backoff
        options.MaxRetryAttempts = 5;
        options.UseExponentialBackoff = true;  // 1s, 2s, 4s, 8s, 16s...
        options.RetryDelay = TimeSpan.FromSeconds(1);
        
        // Intercepte uniquement les routes API
        options.InterceptPredicate = request => 
    request.RequestUri?.AbsolutePath.StartsWith("/api/") ?? false;
        
        // Affiche le loader uniquement pour les mutations
        options.ShowLoaderPredicate = request => 
          request.Method != HttpMethod.Get;
        
   // Callback avant chaque retry
        options.OnRetry = async (attempt, exception, delay) =>
   {
      Console.WriteLine($"Tentative {attempt} après {delay.TotalSeconds}s");
        };
        
 // Active le logging détaillé en développement
     options.EnableDetailedLogging = builder.Environment.IsDevelopment();
    });
```

?? **[Voir la documentation complète des options](./docs/ADVANCED_CONFIGURATION.md)**

### Utilisation

#### Avec interception HTTP (Automatique)

```csharp
@inject IHttpClientFactory HttpClientFactory

<button @onclick="FetchData">Charger les donnees</button>

@code {
  private async Task FetchData()
    {
        // Le loader s'affiche automatiquement !
  var client = HttpClientFactory.CreateClient("BlazorFlexLoader");
        var response = await client.GetAsync("/api/data");
        
    // Le loader se masque automatiquement a la fin
        // Retry automatique en cas d'erreur !
    }
}
```

#### Methodes manuelles Show/Close

```csharp
@inject LoaderService LoaderService

<button @onclick="ShowLoader">Afficher le loader</button>

@code {
    private async Task ShowLoader()
  {
        LoaderService.Show(); // Affiche le SVG anime par defaut
   
        try
        {
            await SomeAsyncOperation();
        }
  finally
  {
      LoaderService.Close(); // Masque le loader
     }
    }
}
```

#### Loader par defaut vs personnalise

```razor
<!-- Loader par defaut avec SVG anime -->
<FlexLoader />

<!-- Loader avec image personnalisee -->
<FlexLoader ImagePath="/images/custom-loader.gif" />

<!-- Loader avec contenu custom -->
<FlexLoader>
    <CustomContent>
        <div class="spinner-border" role="status"></div>
    </CustomContent>
</FlexLoader>
```

#### Methodes Increment/Decrement (gestion avancee)

```csharp
LoaderService.Increment(); // Compter +1
LoaderService.Decrement(); // Compter -1
LoaderService.Reset();     // Forcer fermeture
```

### Parametres du composant

| Parametre | Type | Defaut | Description |
|-----------|------|--------|-------------|
| `ImagePath` | `string?` | `null` | Chemin vers l'image (si null = SVG par defaut) |
| `Text` | `string?` | `"Chargement..."` | Texte affiche pendant le chargement |
| `BackgroundColor` | `string` | `"rgba(255,255,255,0.75)"` | Couleur de fond de l'overlay |
| `TextColor` | `string` | `"#333"` | Couleur du texte |
| `ImageHeight` | `string?` | `"120px"` | Hauteur de l'image (n'affecte pas le SVG) |
| `UseAbsolutePosition` | `bool` | `true` | Position absolue ou fixe |
| `CustomContent` | `RenderFragment?` | `null` | Contenu personnalise |
| `AutoClose` | `bool` | `true` | Fermeture automatique avec delai |
| `AutoCloseDelay` | `int` | `300` | Delai avant fermeture (ms) |
| `CloseOnOverlayClick` | `bool` | `false` | Fermer au clic sur l'overlay |

### API du LoaderService

```csharp
public class LoaderService
{
    public bool IsLoading { get; }
    
    // Methodes simples
    public void Show();       // Afficher
    public void Close();      // Masquer
    
    // Methodes avancees
    public void Increment();  // Compter +1
    public void Decrement();  // Compter -1
    public void Reset();      // Forcer fermeture
    
    public event EventHandler? OnChange;
}
```

### API des Extensions

```csharp
// Configuration basique
services.AddBlazorFlexLoader();

// Configuration avec interception HTTP (options par défaut)
services.AddBlazorFlexLoaderWithHttpInterceptor(client =>
{
    client.BaseAddress = new Uri("https://api.example.com");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// Configuration avec interception HTTP + options personnalisées ??
services.AddBlazorFlexLoaderWithHttpInterceptor(
    client =>
    {
        client.BaseAddress = new Uri("https://api.example.com");
    },
    options =>
  {
   options.MaxRetryAttempts = 5;
        options.UseExponentialBackoff = true;
      options.RetryDelay = TimeSpan.FromSeconds(2);
 options.RetryOnStatusCodes = new HashSet<HttpStatusCode>
  {
        HttpStatusCode.InternalServerError,
    HttpStatusCode.BadGateway,
      HttpStatusCode.ServiceUnavailable
        };
        options.InterceptPredicate = request => 
            request.RequestUri?.AbsolutePath.StartsWith("/api/") ?? false;
    });
```

### Options de configuration avancées ??

| Option | Type | Défaut | Description |
|--------|------|--------|-------------|
| `MaxRetryAttempts` | `int` | `3` | Nombre maximum de tentatives |
| `RetryDelay` | `TimeSpan` | `1s` | Délai de base entre les tentatives |
| `UseExponentialBackoff` | `bool` | `true` | Augmentation exponentielle du délai |
| `RetryOnStatusCodes` | `HashSet<HttpStatusCode>` | `[500,502,503,504,408]` | Codes HTTP déclenchant un retry |
| `RetryOnTimeout` | `bool` | `true` | Retry en cas de timeout |
| `InterceptPredicate` | `Func<HttpRequestMessage, bool>?` | `null` | Filtre les requêtes à intercepter |
| `ShowLoaderPredicate` | `Func<HttpRequestMessage, bool>?` | `null` | Filtre l'affichage du loader |
| `OnRetry` | `Func<int, Exception?, TimeSpan, Task>?` | `null` | Callback avant chaque retry |
| `EnableDetailedLogging` | `bool` | `false` | Active le logging détaillé |

### Fonctionnement de l'intercepteur HTTP

L'`HttpCallInterceptorHandler` intercepte automatiquement toutes les requetes HTTP et:

1. **Affiche le loader** au debut de chaque requete (filtrable avec `ShowLoaderPredicate`)
2. **Masque le loader** a la fin (succes ou echec)
3. **Reessaie automatiquement** selon la configuration :
   - Codes HTTP configurables (défaut: 500, 502, 503, 504, 408)
   - `HttpRequestException` (probleme reseau)
   - Timeouts (si `RetryOnTimeout = true`)
4. **Clone les requetes** pour permettre les retries multiples
5. **Log les événements** avec ILogger (optionnel)
6. **Applique exponential backoff** pour éviter la surcharge (si activé)

### Logging ??

Le handler utilise `ILogger<HttpCallInterceptorHandler>` automatiquement si disponible :

```csharp
// Dans Program.cs
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);
```

**Logs générés :**
- `LogInformation` : Début/fin de requête avec durée
- `LogWarning` : Tentatives de retry
- `LogError` : Échecs après toutes les tentatives
- `LogDebug` : Détails techniques (si `EnableDetailedLogging = true`)

### Exemples pratiques

#### N'intercepter que les appels API

```csharp
options.InterceptPredicate = request => 
    request.RequestUri?.AbsolutePath.StartsWith("/api/") ?? false;
```

#### Loader uniquement pour les mutations (POST/PUT/DELETE)

```csharp
options.ShowLoaderPredicate = request => 
    request.Method == HttpMethod.Post ||
    request.Method == HttpMethod.Put ||
    request.Method == HttpMethod.Delete;
```

#### Notification avant chaque retry

```csharp
@inject INotificationService Notifications

options.OnRetry = async (attempt, exception, delay) =>
{
    await Notifications.ShowWarning($"Tentative {attempt}...");
};
```

### Licence

MIT

---

## English

A flexible and reusable Blazor component for displaying loading indicators with support for custom images, text, and custom content. **Includes animated SVG by default!**

### Features

- **Built-in animated SVG** - Zero configuration required!
- **Automatic HTTP interception** - Shows loader during HTTP requests
- **Automatic retry** - Retries failed requests up to 3 times
- **Exponential backoff** - Smart delay between retry attempts
- **Advanced configuration** - Customizable options for retry and interception
- **Integrated logging** - Full traceability with ILogger
- **Conditional filtering** - Intercept only specific routes or methods
- Global loading indicator for Blazor applications
- Support for custom images (GIF, SVG, PNG, etc.)
- Customizable loading text
- Custom content with RenderFragment
- Absolute or fixed positioning
- Customizable colors and styles
- Configurable auto-close
- Close on overlay click
- Compatible with .NET 9
- Automated CI/CD with GitHub Actions

### Installation

```bash
dotnet add package Blazor.FlexLoader
```

### Setup

#### Option 1: Basic Setup

##### 1. Register the service in `Program.cs`

```csharp
using Blazor.FlexLoader.Extensions;

builder.Services.AddBlazorFlexLoader();
```

##### 2. Add imports in `_Imports.razor`

```razor
@using Blazor.FlexLoader.Components
@using Blazor.FlexLoader.Services
```

##### 3. Add the component to your layout

```razor
<FlexLoader />
```

**That's it! The loader will automatically display a professional animated SVG.**

#### Option 2: Setup with HTTP Interception (Recommended)

##### 1. Register services with HTTP interception in `Program.cs`

```csharp
using Blazor.FlexLoader.Extensions;

// Configure the loader with automatic HTTP request interception
builder.Services.AddBlazorFlexLoaderWithHttpInterceptor(client =>
{
    client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
    // Other HttpClient configurations...
});
```

##### 2. Add imports in `_Imports.razor`

```razor
@using Blazor.FlexLoader.Components
@using Blazor.FlexLoader.Services
```

##### 3. Add the component to your layout

```razor
<FlexLoader />
```

**Benefits:**
- ? Loader displays automatically during **all** HTTP requests
- ? Automatic retry (3 attempts) on HTTP 500 errors or `HttpRequestException`
- ? No need to manually call `Show()` and `Close()`
- ? Centralized network error handling

#### Option 3: Advanced Setup with Custom Options ??

```csharp
using Blazor.FlexLoader.Extensions;

builder.Services.AddBlazorFlexLoaderWithHttpInterceptor(
    client =>
    {
        client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
 client.Timeout = TimeSpan.FromSeconds(30);
    },
    options =>
    {
   // Retry with exponential backoff
        options.MaxRetryAttempts = 5;
      options.UseExponentialBackoff = true;  // 1s, 2s, 4s, 8s, 16s...
  options.RetryDelay = TimeSpan.FromSeconds(1);
        
        // Intercept only API routes
        options.InterceptPredicate = request => 
        request.RequestUri?.AbsolutePath.StartsWith("/api/") ?? false;
        
        // Show loader only for mutations
        options.ShowLoaderPredicate = request => 
   request.Method != HttpMethod.Get;
        
    // Callback before each retry
        options.OnRetry = async (attempt, exception, delay) =>
        {
            Console.WriteLine($"Retry attempt {attempt} after {delay.TotalSeconds}s");
        };
        
// Enable detailed logging in development
        options.EnableDetailedLogging = builder.Environment.IsDevelopment();
    });
```

?? **[See full options documentation](./docs/ADVANCED_CONFIGURATION.md)**

### Usage

#### With HTTP Interception (Automatic)

```csharp
@inject IHttpClientFactory HttpClientFactory

<button @onclick="FetchData">Load data</button>

@code {
    private async Task FetchData()
    {
        // Loader displays automatically!
   var client = HttpClientFactory.CreateClient("BlazorFlexLoader");
        var response = await client.GetAsync("/api/data");
        
        // Loader hides automatically when done
 // Automatic retry on errors!
    }
}
```

#### Manual Show/Close methods

```csharp
@inject LoaderService LoaderService

<button @onclick="ShowLoader">Show loader</button>

@code {
    private async Task ShowLoader()
    {
   LoaderService.Show(); // Shows the default animated SVG
      
   try
      {
 await SomeAsyncOperation();
        }
    finally
        {
            LoaderService.Close(); // Hide the loader
        }
    }
}
```

#### Loader par defaut vs personnalise

```razor
<!-- Loader par defaut avec SVG anime -->
<FlexLoader />

<!-- Loader avec image personnalisee -->
<FlexLoader ImagePath="/images/custom-loader.gif" />

<!-- Loader avec contenu custom -->
<FlexLoader>
    <CustomContent>
        <div class="spinner-border" role="status"></div>
    </CustomContent>
</FlexLoader>
```

#### Methodes Increment/Decrement (gestion avancee)

```csharp
LoaderService.Increment(); // Compter +1
LoaderService.Decrement(); // Compter -1
LoaderService.Reset();     // Forcer fermeture
```

### Parametres du composant

| Parametre | Type | Defaut | Description |
|-----------|------|--------|-------------|
| `ImagePath` | `string?` | `null` | Chemin vers l'image (si null = SVG par defaut) |
| `Text` | `string?` | `"Chargement..."` | Texte affiche pendant le chargement |
| `BackgroundColor` | `string` | `"rgba(255,255,255,0.75)"` | Couleur de fond de l'overlay |
| `TextColor` | `string` | `"#333"` | Couleur du texte |
| `ImageHeight` | `string?` | `"120px"` | Hauteur de l'image (n'affecte pas le SVG) |
| `UseAbsolutePosition` | `bool` | `true` | Position absolue ou fixe |
| `CustomContent` | `RenderFragment?` | `null` | Contenu personnalise |
| `AutoClose` | `bool` | `true` | Fermeture automatique avec delai |
| `AutoCloseDelay` | `int` | `300` | Delai avant fermeture (ms) |
| `CloseOnOverlayClick` | `bool` | `false` | Fermer au clic sur l'overlay |

### API du LoaderService

```csharp
public class LoaderService
{
    public bool IsLoading { get; }
    
    // Methodes simples
    public void Show();       // Afficher
    public void Close();      // Masquer
    
    // Methodes avancees
    public void Increment();  // Compter +1
    public void Decrement();  // Compter -1
    public void Reset();      // Forcer fermeture
    
    public event EventHandler? OnChange;
}
```

### API des Extensions

```csharp
// Basic configuration
services.AddBlazorFlexLoader();

// HTTP interception with default options
services.AddBlazorFlexLoaderWithHttpInterceptor(client =>
{
 client.BaseAddress = new Uri("https://api.example.com");
    client.Timeout = TimeSpan.FromSeconds(30);
});

// HTTP interception with custom options ??
services.AddBlazorFlexLoaderWithHttpInterceptor(
    client =>
    {
        client.BaseAddress = new Uri("https://api.example.com");
    },
    options =>
  {
   options.MaxRetryAttempts = 5;
        options.UseExponentialBackoff = true;
      options.RetryDelay = TimeSpan.FromSeconds(2);
 options.RetryOnStatusCodes = new HashSet<HttpStatusCode>
  {
        HttpStatusCode.InternalServerError,
    HttpStatusCode.BadGateway,
      HttpStatusCode.ServiceUnavailable
        };
        options.InterceptPredicate = request => 
            request.RequestUri?.AbsolutePath.StartsWith("/api/") ?? false;
    });
```

### Advanced Configuration Options ??

| Option | Type | Default | Description |
|--------|------|---------|-------------|
| `MaxRetryAttempts` | `int` | `3` | Maximum number of retry attempts |
| `RetryDelay` | `TimeSpan` | `1s` | Base delay between attempts |
| `UseExponentialBackoff` | `bool` | `true` | Exponential delay increase |
| `RetryOnStatusCodes` | `HashSet<HttpStatusCode>` | `[500,502,503,504,408]` | HTTP codes triggering retry |
| `RetryOnTimeout` | `bool` | `true` | Retry on timeout |
| `InterceptPredicate` | `Func<HttpRequestMessage, bool>?` | `null` | Filter requests to intercept |
| `ShowLoaderPredicate` | `Func<HttpRequestMessage, bool>?` | `null` | Filter loader display |
| `OnRetry` | `Func<int, Exception?, TimeSpan, Task>?` | `null` | Callback before each retry |
| `EnableDetailedLogging` | `bool` | `false` | Enable detailed logging |

### How HTTP Interceptor Works

The `HttpCallInterceptorHandler` automatically intercepts all HTTP requests and:

1. **Shows the loader** at the start of each request (filterable with `ShowLoaderPredicate`)
2. **Hides the loader** at the end (success or failure)
3. **Automatically retries** according to configuration:
   - Configurable HTTP codes (default: 500, 502, 503, 504, 408)
   - `HttpRequestException` (network issues)
   - Timeouts (if `RetryOnTimeout = true`)
4. **Clones requests** to allow multiple retries
5. **Logs events** with ILogger (optional)
6. **Applies exponential backoff** to avoid overload (if enabled)

### Logging ??

The handler uses `ILogger<HttpCallInterceptorHandler>` automatically if available:

```csharp
// In Program.cs
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);
```

**Generated logs:**
- `LogInformation`: Request start/end with duration
- `LogWarning`: Retry attempts
- `LogError`: Failures after all attempts
- `LogDebug`: Technical details (if `EnableDetailedLogging = true`)

### Practical Examples

#### Intercept only API calls

```csharp
options.InterceptPredicate = request => 
 request.RequestUri?.AbsolutePath.StartsWith("/api/") ?? false;
```

#### Loader only for mutations (POST/PUT/DELETE)

```csharp
options.ShowLoaderPredicate = request => 
    request.Method == HttpMethod.Post ||
    request.Method == HttpMethod.Put ||
    request.Method == HttpMethod.Delete;
```

#### Notification before each retry

```csharp
@inject INotificationService Notifications

options.OnRetry = async (attempt, exception, delay) =>
{
    await Notifications.ShowWarning($"Tentative {attempt}...");
};
```

### License

MIT