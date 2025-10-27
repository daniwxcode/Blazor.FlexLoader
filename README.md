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

// Configuration avec interception HTTP + retry automatique
services.AddBlazorFlexLoaderWithHttpInterceptor(client =>
{
    client.BaseAddress = new Uri("https://api.example.com");
    client.Timeout = TimeSpan.FromSeconds(30);
});
```

### Fonctionnement de l'intercepteur HTTP

L'`HttpCallInterceptorHandler` intercepte automatiquement toutes les requetes HTTP et:

1. **Affiche le loader** au debut de chaque requete
2. **Masque le loader** a la fin (succes ou echec)
3. **Reessaie automatiquement** jusqu'a 3 fois en cas de:
   - `HttpRequestException` (probleme reseau)
   - Reponse HTTP 500 (erreur serveur)
4. **Clone les requetes** pour permettre les retries multiples

### Licence

MIT

---

## English

A flexible and reusable Blazor component for displaying loading indicators with support for custom images, text, and custom content. **Includes animated SVG by default!**

### Features

- **Built-in animated SVG** - Zero configuration required!
- **Automatic HTTP interception** - Shows loader during HTTP requests
- **Automatic retry** - Retries failed requests up to 3 times
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

#### Default vs Custom loader

```razor
<!-- Default loader with animated SVG -->
<FlexLoader />

<!-- Custom image loader -->
<FlexLoader ImagePath="/images/custom-loader.gif" />

<!-- Custom content loader -->
<FlexLoader>
    <CustomContent>
        <div class="spinner-border" role="status"></div>
    </CustomContent>
</FlexLoader>
```

#### Increment/Decrement methods (advanced management)

```csharp
LoaderService.Increment(); // Count +1
LoaderService.Decrement(); // Count -1
LoaderService.Reset();     // Force close
```

### Component Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ImagePath` | `string?` | `null` | Path to image (if null = default SVG) |
| `Text` | `string?` | `"Loading..."` | Text displayed during loading |
| `BackgroundColor` | `string` | `"rgba(255,255,255,0.75)"` | Overlay background color |
| `TextColor` | `string` | `"#333"` | Text color |
| `ImageHeight` | `string?` | `"120px"` | Image height (doesn't affect SVG) |
| `UseAbsolutePosition` | `bool` | `true` | Absolute or fixed position |
| `CustomContent` | `RenderFragment?` | `null` | Custom content |
| `AutoClose` | `bool` | `true` | Auto close with delay |
| `AutoCloseDelay` | `int` | `300` | Delay before closing (ms) |
| `CloseOnOverlayClick` | `bool` | `false` | Close on overlay click |

### LoaderService API

```csharp
public class LoaderService
{
    public bool IsLoading { get; }
    
    // Simple methods
    public void Show();       // Display
    public void Close();      // Hide
    
    // Advanced methods
    public void Increment();  // Count +1
    public void Decrement();  // Count -1
    public void Reset();      // Force close
    
    public event EventHandler? OnChange;
}
```

### Extensions API

```csharp
// Basic configuration
services.AddBlazorFlexLoader();

// Configuration with HTTP interception + automatic retry
services.AddBlazorFlexLoaderWithHttpInterceptor(client =>
{
    client.BaseAddress = new Uri("https://api.example.com");
    client.Timeout = TimeSpan.FromSeconds(30);
});
```

### How HTTP Interceptor Works

The `HttpCallInterceptorHandler` automatically intercepts all HTTP requests and:

1. **Shows the loader** at the start of each request
2. **Hides the loader** at the end (success or failure)
3. **Automatically retries** up to 3 times on:
   - `HttpRequestException` (network issues)
   - HTTP 500 response (server error)
4. **Clones requests** to allow multiple retries

### License

MIT