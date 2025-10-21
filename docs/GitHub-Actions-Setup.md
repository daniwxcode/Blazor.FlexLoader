# GitHub Actions Configuration

## ?? Secrets Required

Pour que les workflows fonctionnent correctement, vous devez configurer les secrets suivants dans votre repository GitHub :

### Settings ? Secrets and variables ? Actions

#### Required Secrets:
- **`NUGET_API_KEY`** - Clé API pour publier sur NuGet.org
  - ?? Obtenez votre clé sur : https://www.nuget.org/account/apikeys
  - ?? Scope requis : `Push new packages and package versions`

#### Optional Secrets:
- **`GITHUB_TOKEN`** - Généré automatiquement par GitHub (pas besoin de le configurer)

## ?? Workflows Disponibles

### 1. ?? Continuous Integration (`ci.yml`)
- **Déclencheur** : Push/PR sur `main` ou `develop`
- **Actions** :
  - Build et validation du code
  - Vérification du formatage
  - Tests (si présents)
  - Validation de la documentation
  - Création d'artefacts de test

### 2. ?? Build and Publish NuGet (`nuget-publish.yml`)
- **Déclencheur** : Tags `v*.*.*` (ex: `v1.2.0`)
- **Actions** :
  - Build et tests
  - Création du package NuGet
  - Publication sur NuGet.org
  - Création d'une release GitHub

### 3. ?? Update Dependencies (`dependency-update.yml`)
- **Déclencheur** : Scheduled (lundi 2h) + manuel
- **Actions** :
  - Vérification des packages obsolètes
  - Mise à jour automatique
  - Création d'une PR

## ?? Utilisation

### Publier une nouvelle version :

1. **Mettre à jour la version** dans `Blazor.FlexLoader.csproj`
2. **Mettre à jour** le `CHANGELOG.md`
3. **Commit et push** les changements
4. **Créer un tag** :
   ```bash
   git tag v1.3.0
   git push origin v1.3.0
   ```
5. **GitHub Actions** se charge automatiquement du reste !

### Workflow manuel :

Vous pouvez déclencher manuellement les workflows depuis l'onglet "Actions" de votre repository GitHub.

## ?? Configuration avancée

### Variables d'environnement globales :
- `DOTNET_VERSION`: Version de .NET utilisée (actuellement `9.0.x`)
- `PROJECT_PATH`: Chemin vers le fichier projet

### Personnalisation :
- Modifiez les workflows dans `.github/workflows/`
- Ajustez les déclencheurs selon vos besoins
- Configurez des notifications Slack/Teams si nécessaire

## ?? Badges pour le README

Ajoutez ces badges à votre README.md :

```markdown
[![CI](https://github.com/daniwxcode/Blazor.FlexLoader/actions/workflows/ci.yml/badge.svg)](https://github.com/daniwxcode/Blazor.FlexLoader/actions/workflows/ci.yml)
[![NuGet](https://img.shields.io/nuget/v/Blazor.FlexLoader.svg)](https://www.nuget.org/packages/Blazor.FlexLoader/)
[![Downloads](https://img.shields.io/nuget/dt/Blazor.FlexLoader.svg)](https://www.nuget.org/packages/Blazor.FlexLoader/)
```