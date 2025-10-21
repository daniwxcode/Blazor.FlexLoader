# Assets - Blazor.FlexLoader Icons

## ?? Ic�nes disponibles

### ?? Package NuGet Icon
- **`icon-static.svg`** - Ic�ne principale du package NuGet (128x128)
  - Couleurs : D�grad� Blazor violet (#512BD4 ? #8A2BE2)
  - Loader rings avec d�grad� color�
  - Format : SVG statique optimis� pour NuGet

### ?? Animated Icon
- **`icon.svg`** - Version anim�e pour documentation/web (128x128)
  - Animations CSS des anneaux de chargement
  - Effets de pulsation sur les points
  - Id�al pour les d�mos et documentation

### ?? Favicon
- **`favicon.svg`** - Ic�ne simplifi�e (32x32)
  - Version minimaliste pour favicons
  - Optimis�e pour petites tailles

## ?? Palette de couleurs

### Couleurs principales :
- **Blazor Purple** : `#512BD4` ? `#8A2BE2` (d�grad�)
- **Loader Gradient** : `#FF6B6B` ? `#4ECDC4` ? `#45B7D1`
- **Accents** : `#FFFFFF` (blanc pour contraste)

### Design inspir� de :
- ?? **Blazor branding** - Couleurs officielles Microsoft
- ?? **Spinners modernes** - Anneaux de chargement fluides
- ? **UI contemporaine** - Design minimaliste et professionnel

## ?? Utilisation

### Dans NuGet :
L'ic�ne `icon-static.svg` s'affiche automatiquement dans :
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

Pour modifier les ic�nes :
1. �diter les fichiers SVG dans `assets/`
2. Maintenir les proportions 1:1 (carr�)
3. Tester la lisibilit� aux petites tailles
4. Respecter la palette de couleurs Blazor

## ?? Sp�cifications techniques

- **Format** : SVG (vectoriel, scalable)
- **Dimensions** : 128x128px (recommand� NuGet)
- **Poids** : ~2KB (optimis�)
- **Compatibilit�** : Tous navigateurs modernes
- **Transparence** : Arri�re-plan transparent