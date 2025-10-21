# Assets - Blazor.FlexLoader Icons

## ?? Icônes disponibles

### ?? Package NuGet Icon
- **`icon-static.svg`** - Icône principale du package NuGet (128x128)
  - Couleurs : Dégradé Blazor violet (#512BD4 ? #8A2BE2)
  - Loader rings avec dégradé coloré
  - Format : SVG statique optimisé pour NuGet

### ?? Animated Icon
- **`icon.svg`** - Version animée pour documentation/web (128x128)
  - Animations CSS des anneaux de chargement
  - Effets de pulsation sur les points
  - Idéal pour les démos et documentation

### ?? Favicon
- **`favicon.svg`** - Icône simplifiée (32x32)
  - Version minimaliste pour favicons
  - Optimisée pour petites tailles

## ?? Palette de couleurs

### Couleurs principales :
- **Blazor Purple** : `#512BD4` ? `#8A2BE2` (dégradé)
- **Loader Gradient** : `#FF6B6B` ? `#4ECDC4` ? `#45B7D1`
- **Accents** : `#FFFFFF` (blanc pour contraste)

### Design inspiré de :
- ?? **Blazor branding** - Couleurs officielles Microsoft
- ?? **Spinners modernes** - Anneaux de chargement fluides
- ? **UI contemporaine** - Design minimaliste et professionnel

## ?? Utilisation

### Dans NuGet :
L'icône `icon-static.svg` s'affiche automatiquement dans :
- NuGet.org package listing
- Visual Studio Package Manager
- dotnet CLI search results

### Dans la documentation :
```markdown
![Blazor.FlexLoader](./assets/icon.svg)
```

### Pour le web :
```html
<link rel="icon" type="image/svg+xml" href="./assets/favicon.svg">
```

## ?? Modifications

Pour modifier les icônes :
1. Éditer les fichiers SVG dans `assets/`
2. Maintenir les proportions 1:1 (carré)
3. Tester la lisibilité aux petites tailles
4. Respecter la palette de couleurs Blazor

## ?? Spécifications techniques

- **Format** : SVG (vectoriel, scalable)
- **Dimensions** : 128x128px (recommandé NuGet)
- **Poids** : ~2KB (optimisé)
- **Compatibilité** : Tous navigateurs modernes
- **Transparence** : Arrière-plan transparent