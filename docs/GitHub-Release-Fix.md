# Instructions pour cr�er une release GitHub manuelle

## ?? Probl�me r�solu
Le workflow GitHub Actions avait un probl�me de permissions pour cr�er les releases (erreur 403).

## ? Corrections apport�es

### 1. Permissions ajout�es au workflow
```yaml
permissions:
  contents: write
  packages: write
  actions: read
```

### 2. Mise � jour de l'action
- **Avant** : `softprops/action-gh-release@v1`
- **Apr�s** : `softprops/action-gh-release@v2`

### 3. Option de release principale
```yaml
make_latest: true
```

## ?? Cr�ation manuelle de release (si n�cessaire)

### Pour la version v1.3.1 :
1. Aller sur https://github.com/daniwxcode/Blazor.FlexLoader/releases
2. Cliquer sur "Create a new release"
3. **Tag** : v1.3.1
4. **Title** : Release v1.3.1
5. **Description** :
```markdown
## ?? Blazor.FlexLoader v1.3.1

### ?? NuGet Package
```bash
dotnet add package Blazor.FlexLoader --version 1.3.1
```

### ?? Installation
```csharp
// Program.cs
builder.Services.AddBlazorFlexLoader();
```

### ?? Links
- ?? [NuGet Package](https://www.nuget.org/packages/Blazor.FlexLoader/1.3.1)
- ?? [Documentation](https://github.com/daniwxcode/Blazor.FlexLoader#readme)
- ?? [Detailed Guide](https://github.com/daniwxcode/Blazor.FlexLoader/blob/main/docs/LoaderService-Documentation.md)

### ? Changes
- Fix README icon display with GitHub-compatible SVG
- Improved visual presentation
- Better icon rendering on GitHub

See [CHANGELOG.md](https://github.com/daniwxcode/Blazor.FlexLoader/blob/main/CHANGELOG.md) for detailed changes.
```

## ?? Test avec v1.3.2
La version 1.3.2 va tester le workflow corrig� automatiquement.