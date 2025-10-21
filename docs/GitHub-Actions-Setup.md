# GitHub Actions Configuration

## ?? Secrets Required

Pour que les workflows fonctionnent correctement, vous devez configurer les secrets suivants dans votre repository GitHub :

### Settings ? Secrets and variables ? Actions

#### Required Secrets:
- **`NUGET_API_KEY`** - Cl� API pour publier sur NuGet.org
  - ?? Obtenez votre cl� sur : https://www.nuget.org/account/apikeys
  - ?? Scope requis : `Push new packages and package versions`

#### Optional Secrets:
- **`GITHUB_TOKEN`** - G�n�r� automatiquement par GitHub (pas besoin de le configurer)

## ?? Workflows Disponibles

### 1. ?? Continuous Integration (`ci.yml`)
- **D�clencheur** : Push/PR sur `main` ou `develop`
- **Actions** :
  - Build et validation du code
  - V�rification du formatage
  - Tests (si pr�sents)
  - Validation de la documentation
  - Cr�ation d'artefacts de test

### 2. ?? Build and Publish NuGet (`nuget-publish.yml`)
- **D�clencheur** : Tags `v*.*.*` (ex: `v1.2.0`)
- **Actions** :
  - Build et tests
  - Cr�ation du package NuGet
  - Publication sur NuGet.org
  - Cr�ation d'une release GitHub

### 3. ?? Update Dependencies (`dependency-update.yml`)
- **D�clencheur** : Scheduled (lundi 2h) + manuel
- **Actions** :
  - V�rification des packages obsol�tes
  - Mise � jour automatique
  - Cr�ation d'une PR

## ?? Utilisation

### Publier une nouvelle version :

1. **Mettre � jour la version** dans `Blazor.FlexLoader.csproj`
2. **Mettre � jour** le `CHANGELOG.md`
3. **Commit et push** les changements
4. **Cr�er un tag** :
   ```bash
   git tag v1.3.0
   git push origin v1.3.0
   ```
5. **GitHub Actions** se charge automatiquement du reste !

### Workflow manuel :

Vous pouvez d�clencher manuellement les workflows depuis l'onglet "Actions" de votre repository GitHub.

## ?? Configuration avanc�e

### Variables d'environnement globales :
- `DOTNET_VERSION`: Version de .NET utilis�e (actuellement `9.0.x`)
- `PROJECT_PATH`: Chemin vers le fichier projet

### Personnalisation :
- Modifiez les workflows dans `.github/workflows/`
- Ajustez les d�clencheurs selon vos besoins
- Configurez des notifications Slack/Teams si n�cessaire

## ?? Badges pour le README

Ajoutez ces badges � votre README.md :

```markdown
[![CI](https://github.com/daniwxcode/Blazor.FlexLoader/actions/workflows/ci.yml/badge.svg)](https://github.com/daniwxcode/Blazor.FlexLoader/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Blazor.FlexLoader.svg)](https://www.nuget.org/packages/Blazor.FlexLoader/)
[![Downloads](https://img.shields.io/nuget/dt/Blazor.FlexLoader.svg)](https://www.nuget.org/packages/Blazor.FlexLoader/)
```