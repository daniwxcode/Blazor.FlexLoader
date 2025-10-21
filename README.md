# Blazor.FlexLoader

[![CI](https://github.com/daniwxcode/Blazor.FlexLoader/actions/workflows/ci.yml/badge.svg)](https://github.com/daniwxcode/Blazor.FlexLoader/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Blazor.FlexLoader.svg)](https://www.nuget.org/packages/Blazor.FlexLoader/)
[![Downloads](https://img.shields.io/nuget/dt/Blazor.FlexLoader.svg)](https://www.nuget.org/packages/Blazor.FlexLoader/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](https://opensource.org/licenses/MIT)

<div align="center">
  <img src="./assets/icon.svg" alt="Blazor.FlexLoader" width="128" height="128" />
</div>

[???? Français](#français) | [???? English](#english)

---

## Français

Un composant Blazor flexible et réutilisable pour afficher des indicateurs de chargement avec support pour des images personnalisées, du texte et du contenu custom.

### ?? Fonctionnalités

- ? Indicateur de chargement global pour les applications Blazor
- ? Support d'images personnalisées (GIF, SVG, PNG, etc.)
- ? Texte de chargement personnalisable
- ? Contenu custom avec RenderFragment
- ? Positionnement absolu ou fixe
- ? Couleurs et styles personnalisables
- ? Fermeture automatique configurable
- ? Fermeture au clic sur l'overlay
- ? Compatible .NET 9
- ? CI/CD automatisé avec GitHub Actions

### ?? Installation

```bash
dotnet add package Blazor.FlexLoader
```

### ??? Configuration

#### 1. Enregistrer le service dans `Program.cs`

```csharp
using Blazor.FlexLoader.Extensions;

builder.Services.AddBlazorFlexLoader();
```

#### 2. Ajouter les imports dans `_Imports.razor`

```razor
@using Blazor.FlexLoader.Components
@using Blazor.FlexLoader.Services
```

#### 3. Ajouter le composant dans votre layout

```razor
<FlexLoader />
```

### ?? Utilisation

#### Méthodes simples Show/Close

```csharp
@inject LoaderService LoaderService

<button @onclick="ShowLoader">Afficher le loader</button>

@code {
    private async Task ShowLoader()
    {
        LoaderService.Show(); // Afficher le loader
        
        try
        {
            await SomeAsyncOperation();
        }
        finally
        {
            LoaderService.Close(); // Masquer le loader
        }
    }
}
```

#### Méthodes Increment/Decrement (gestion avancée)

```csharp
LoaderService.Increment(); // Compter +1
LoaderService.Decrement(); // Compter -1
LoaderService.Reset();     // Forcer fermeture
```

### ?? Paramètres du composant

| Paramètre | Type | Défaut | Description |
|-----------|------|--------|-------------|
| `ImagePath` | `string?` | `null` | Chemin vers l'image de chargement |
| `Text` | `string?` | `"Chargement..."` | Texte affiché pendant le chargement |
| `BackgroundColor` | `string` | `"rgba(255,255,255,0.75)"` | Couleur de fond de l'overlay |
| `TextColor` | `string` | `"#333"` | Couleur du texte |
| `ImageHeight` | `string?` | `"120px"` | Hauteur de l'image |
| `UseAbsolutePosition` | `bool` | `true` | Position absolue ou fixe |
| `CustomContent` | `RenderFragment?` | `null` | Contenu personnalisé |
| `AutoClose` | `bool` | `true` | Fermeture automatique avec délai |
| `AutoCloseDelay` | `int` | `300` | Délai avant fermeture (ms) |
| `CloseOnOverlayClick` | `bool` | `false` | Fermer au clic sur l'overlay |

### ?? API du LoaderService

```csharp
public class LoaderService
{
    public bool IsLoading { get; }
    
    // Méthodes simples
    public void Show();       // Afficher
    public void Close();      // Masquer
    
    // Méthodes avancées
    public void Increment();  // Compter +1
    public void Decrement();  // Compter -1
    public void Reset();      // Forcer fermeture
    
    public event EventHandler? OnChange;
}
```

### ?? Licence

MIT

---

## English

A flexible and reusable Blazor component for displaying loading indicators with support for custom images, text, and custom content.

### ?? Features

- ? Global loading indicator for Blazor applications
- ? Support for custom images (GIF, SVG, PNG, etc.)
- ? Customizable loading text
- ? Custom content with RenderFragment
- ? Absolute or fixed positioning
- ? Customizable colors and styles
- ? Configurable auto-close
- ? Close on overlay click
- ? Compatible with .NET 9
- ? Automated CI/CD with GitHub Actions

### ?? Installation

```bash
dotnet add package Blazor.FlexLoader
```

### ??? Setup

#### 1. Register the service in `Program.cs`

```csharp
using Blazor.FlexLoader.Extensions;

builder.Services.AddBlazorFlexLoader();
```

#### 2. Add imports in `_Imports.razor`

```razor
@using Blazor.FlexLoader.Components
@using Blazor.FlexLoader.Services
```

#### 3. Add the component to your layout

```razor
<FlexLoader />
```

### ?? Usage

#### Simple Show/Close methods

```csharp
@inject LoaderService LoaderService

<button @onclick="ShowLoader">Show loader</button>

@code {
    private async Task ShowLoader()
    {
        LoaderService.Show(); // Show the loader
        
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

#### Increment/Decrement methods (advanced management)

```csharp
LoaderService.Increment(); // Count +1
LoaderService.Decrement(); // Count -1
LoaderService.Reset();     // Force close
```

### ?? Component Parameters

| Parameter | Type | Default | Description |
|-----------|------|---------|-------------|
| `ImagePath` | `string?` | `null` | Path to the loading image |
| `Text` | `string?` | `"Loading..."` | Text displayed during loading |
| `BackgroundColor` | `string` | `"rgba(255,255,255,0.75)"` | Overlay background color |
| `TextColor` | `string` | `"#333"` | Text color |
| `ImageHeight` | `string?` | `"120px"` | Image height |
| `UseAbsolutePosition` | `bool` | `true` | Absolute or fixed position |
| `CustomContent` | `RenderFragment?` | `null` | Custom content |
| `AutoClose` | `bool` | `true` | Auto close with delay |
| `AutoCloseDelay` | `int` | `300` | Delay before closing (ms) |
| `CloseOnOverlayClick` | `bool` | `false` | Close on overlay click |

### ?? LoaderService API

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

### ?? License

MIT