# Blazor.FlexLoader

Un composant Blazor flexible et réutilisable pour afficher des indicateurs de chargement.

## Installation

```bash
dotnet add package Blazor.FlexLoader
```

## Configuration

### 1. Enregistrer le service dans `Program.cs`

```csharp
using Blazor.FlexLoader.Extensions;

builder.Services.AddBlazorFlexLoader();
```

### 2. Ajouter les imports dans `_Imports.razor`

```razor
@using Blazor.FlexLoader.Components
@using Blazor.FlexLoader.Services
```

### 3. Ajouter le composant dans votre layout

```razor
<FlexLoader />
```

## Utilisation

### Méthodes simples Show/Close

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

### Méthodes Increment/Decrement (gestion avancée)

```csharp
LoaderService.Increment(); // Compter +1
LoaderService.Decrement(); // Compter -1
LoaderService.Reset();     // Forcer fermeture
```

## API du LoaderService

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

## Paramètres

- ImagePath : Chemin vers l'image
- Text : Texte affiché
- BackgroundColor : Couleur de fond
- TextColor : Couleur du texte
- CustomContent : Contenu personnalisé

## License

MIT