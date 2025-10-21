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

```csharp
@inject LoaderService LoaderService

<button @onclick="ShowLoader">Afficher le loader</button>

@code {
    private async Task ShowLoader()
    {
        LoaderService.Increment();
        try
        {
            await SomeAsyncOperation();
        }
        finally
        {
            LoaderService.Decrement();
        }
    }
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