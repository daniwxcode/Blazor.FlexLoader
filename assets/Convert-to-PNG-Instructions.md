# Instructions pour cr�er l'ic�ne PNG

## ?? Objectif
Convertir l'ic�ne SVG en format PNG (128x128px) pour le package NuGet.

## ?? M�thodes de conversion

### Option 1: Convertisseur en ligne (Recommand�)
1. Aller sur https://svgtopng.com/ ou https://convertio.co/svg-png/
2. Upload `assets/icon-static.svg`
3. D�finir la taille : **128x128 pixels**
4. T�l�charger le fichier PNG
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

## ?? Apr�s conversion

1. **V�rifier le fichier PNG** cr�� (128x128px)
2. **Mettre � jour** `Blazor.FlexLoader.csproj` :
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

## ?? Sp�cifications PNG requises

- **Format** : PNG avec transparence
- **Dimensions** : 128x128 pixels exactement
- **Poids** : < 50KB (recommand�)
- **Qualit�** : Haute r�solution pour lisibilit�
- **Background** : Transparent

## ?? Validation visuelle

L'ic�ne doit �tre lisible � diff�rentes tailles :
- ? 128x128px (taille originale)
- ? 64x64px (Visual Studio)
- ? 32x32px (listes)
- ? 16x16px (r�f�rences)

## ?? Notes importantes

- NuGet.org ne supporte que PNG, JPG, JPEG
- SVG n'est pas accept� pour les ic�nes de package
- L'ic�ne appara�tra dans le package manager et sur NuGet.org
- Tester la visibilit� sur fond clair ET fonc�