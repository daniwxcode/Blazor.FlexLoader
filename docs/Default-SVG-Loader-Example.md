# Exemple d'utilisation avec le SVG par d�faut

## Utilisation basique

Quand vous ajoutez simplement `<FlexLoader />` dans votre layout, le composant affichera automatiquement un SVG loader anim� par d�faut.

### 1. Configuration minimale

```razor
@* Dans MainLayout.razor ou App.razor *@
<FlexLoader />

@* Le reste de votre contenu *@
<main>
    @Body
</main>
```

### 2. D�clenchement du loader

```razor
@page "/example"
@inject LoaderService LoaderService

<h3>Exemple avec loader par d�faut</h3>

<button class="btn btn-primary" @onclick="ShowDefaultLoader">
    Afficher le loader par d�faut
</button>

<button class="btn btn-secondary" @onclick="SimulateAsyncOperation">
    Simuler une op�ration async
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

## Caract�ristiques du SVG par d�faut

### Design
- **Inspiration** : Bas� sur l'ic�ne du projet Blazor.FlexLoader
- **Couleurs** : Palette Blazor officielle (#512BD4, #8A2BE2)
- **Animations** : Anneaux rotatifs fluides
- **Taille** : 80x80px optimis�e pour la lisibilit�

### Animations
- **Anneau principal** : Rotation 360� en 2 secondes
- **Anneau secondaire** : Rotation inverse en 1.5 secondes  
- **Points d'accent** : Pulsation altern�e
- **Fluide** : Animations CSS sans �-coups

### Avantages
- ? **Aucune configuration** requise
- ? **Pas de fichier externe** � g�rer
- ? **Design coh�rent** avec Blazor
- ? **Animations fluides** int�gr�es
- ? **Responsive** et scalable

## Personnalisation possible

Si vous voulez personnaliser, vous pouvez toujours :

```razor
<!-- Avec image personnalis�e -->
<FlexLoader ImagePath="/images/custom-loader.gif" />

<!-- Avec texte personnalis� -->
<FlexLoader Text="Veuillez patienter..." />

<!-- Avec contenu compl�tement custom -->
<FlexLoader>
    <CustomContent>
        <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden">Chargement...</span>
        </div>
    </CustomContent>
</FlexLoader>
```

Mais avec la version par d�faut, un simple `<FlexLoader />` suffit pour avoir un loader professionnel !