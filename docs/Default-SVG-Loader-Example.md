# Exemple d'utilisation avec le SVG par défaut

## Utilisation basique

Quand vous ajoutez simplement `<FlexLoader />` dans votre layout, le composant affichera automatiquement un SVG loader animé par défaut.

### 1. Configuration minimale

```razor
@* Dans MainLayout.razor ou App.razor *@
<FlexLoader />

@* Le reste de votre contenu *@
<main>
    @Body
</main>
```

### 2. Déclenchement du loader

```razor
@page "/example"
@inject LoaderService LoaderService

<h3>Exemple avec loader par défaut</h3>

<button class="btn btn-primary" @onclick="ShowDefaultLoader">
    Afficher le loader par défaut
</button>

<button class="btn btn-secondary" @onclick="SimulateAsyncOperation">
    Simuler une opération async
</button>

@code {
    private async Task ShowDefaultLoader()
    {
        LoaderService.Show();
        await Task.Delay(3000); // Simule 3 secondes de chargement
        LoaderService.Close();
    }

    private async Task SimulateAsyncOperation()
    {
        LoaderService.Show();
        try
        {
            // Simulation d'un appel API
            await Task.Delay(2000);
            
            // Simulation d'un traitement
            await Task.Delay(1000);
        }
        finally
        {
            LoaderService.Close();
        }
    }
}
```

## Caractéristiques du SVG par défaut

### Design
- **Inspiration** : Basé sur l'icône du projet Blazor.FlexLoader
- **Couleurs** : Palette Blazor officielle (#512BD4, #8A2BE2)
- **Animations** : Anneaux rotatifs fluides
- **Taille** : 80x80px optimisée pour la lisibilité

### Animations
- **Anneau principal** : Rotation 360° en 2 secondes
- **Anneau secondaire** : Rotation inverse en 1.5 secondes  
- **Points d'accent** : Pulsation alternée
- **Fluide** : Animations CSS sans à-coups

### Avantages
- ? **Aucune configuration** requise
- ? **Pas de fichier externe** à gérer
- ? **Design cohérent** avec Blazor
- ? **Animations fluides** intégrées
- ? **Responsive** et scalable

## Personnalisation possible

Si vous voulez personnaliser, vous pouvez toujours :

```razor
<!-- Avec image personnalisée -->
<FlexLoader ImagePath="/images/custom-loader.gif" />

<!-- Avec texte personnalisé -->
<FlexLoader Text="Veuillez patienter..." />

<!-- Avec contenu complètement custom -->
<FlexLoader>
    <CustomContent>
        <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden">Chargement...</span>
        </div>
    </CustomContent>
</FlexLoader>
```

Mais avec la version par défaut, un simple `<FlexLoader />` suffit pour avoir un loader professionnel !