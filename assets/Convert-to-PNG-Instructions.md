# Instructions pour créer l'icône PNG

## ?? Objectif
Convertir l'icône SVG en format PNG (128x128px) pour le package NuGet.

## ?? Méthodes de conversion

### Option 1: Convertisseur en ligne (Recommandé)
1. Aller sur https://svgtopng.com/ ou https://convertio.co/svg-png/
2. Upload `assets/icon-static.svg`
3. Définir la taille : **128x128 pixels**
4. Télécharger le fichier PNG
5. Renommer en `icon.png`
6. Placer dans le dossier `assets/`

### Option 2: Inkscape (Logiciel gratuit)
```bash
# Installation Inkscape
winget install Inkscape.Inkscape

# Conversion
inkscape --export-type=png --export-width=128 --export-height=128 assets/icon-static.svg --export-filename=assets/icon.png
```

### Option 3: ImageMagick
```bash
# Installation
winget install ImageMagick.ImageMagick

# Conversion
magick convert -background transparent -size 128x128 assets/icon-static.svg assets/icon.png
```

### Option 4: Node.js + sharp
```bash
npm install sharp
node -e "
const sharp = require('sharp');
sharp('assets/icon-static.svg')
  .resize(128, 128)
  .png()
  .toFile('assets/icon.png');
"
```

## ?? Après conversion

1. **Vérifier le fichier PNG** créé (128x128px)
2. **Mettre à jour** `Blazor.FlexLoader.csproj` :
   ```xml
   <PackageIcon>icon.png</PackageIcon>
   ```
3. **Ajouter au projet** :
   ```xml
   <None Include="assets\icon.png" Pack="true" PackagePath="\" />
   ```
4. **Tester la compilation** :
   ```bash
   dotnet pack --configuration Release
   ```

## ?? Spécifications PNG requises

- **Format** : PNG avec transparence
- **Dimensions** : 128x128 pixels exactement
- **Poids** : < 50KB (recommandé)
- **Qualité** : Haute résolution pour lisibilité
- **Background** : Transparent

## ?? Validation visuelle

L'icône doit être lisible à différentes tailles :
- ? 128x128px (taille originale)
- ? 64x64px (Visual Studio)
- ? 32x32px (listes)
- ? 16x16px (références)

## ?? Notes importantes

- NuGet.org ne supporte que PNG, JPG, JPEG
- SVG n'est pas accepté pour les icônes de package
- L'icône apparaîtra dans le package manager et sur NuGet.org
- Tester la visibilité sur fond clair ET foncé