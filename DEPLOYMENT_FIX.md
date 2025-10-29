# ?? Correction du Problème de Déploiement Automatique

## ?? Problème Identifié

Depuis la version 1.4, le package NuGet ne se déploie plus automatiquement sur NuGet.org.

### Causes Identifiées

1. **Conflit entre deux workflows** :
   - `publish.yml` : Se déclenche automatiquement sur chaque push vers `main`
   - `nuget-publish.yml` : Se déclenche uniquement sur les tags `v*.*.*`

2. **Changements sur la branche `develop` non mergés dans `main`** :
   - Les derniers commits sont sur `develop`
   - La branche `main` est en retard
   - Le tag `v1.6.1` existe, mais le code n'est pas sur `main`

3. **Workflow basé sur les tags non déclenché correctement**

---

## ? Solutions Appliquées

### 1. Désactivation du workflow `publish.yml`

Le workflow `publish.yml` a été désactivé pour éviter les conflits et publications non désirées.

**Raison** : Publication automatique sur chaque modification de `main` n'est pas recommandée.

### 2. Amélioration du workflow `nuget-publish.yml`

- ? Support pour l'exécution manuelle (`workflow_dispatch`)
- ? Vérification de la clé API NuGet avant publication
- ? Publication des symboles (`.snupkg`)
- ? Meilleure gestion des erreurs
- ? Logs plus détaillés avec emojis

### 3. Création d'un guide de déploiement

Un guide complet (`DEPLOYMENT_GUIDE.md`) a été créé avec :
- Processus de publication recommandé
- Configuration requise
- Checklist de publication
- Section de dépannage

### 4. Script PowerShell de release automatisé

Un script `release.ps1` a été créé pour automatiser :
- Vérification de la branche et des changements
- Mise à jour de la version dans `.csproj`
- Build et tests
- Création du package
- Commit, merge et push
- Création et push du tag

---

## ?? Processus de Publication Recommandé

### Méthode 1 : Utiliser le script PowerShell (Recommandé)

```powershell
# Publication d'une nouvelle version
.\release.ps1 -Version 1.7.0 -ReleaseNotes "Correction du déploiement automatique"

# Dry run (test sans commit)
.\release.ps1 -Version 1.7.0 -DryRun

# Sans tests (plus rapide pour les hotfixes)
.\release.ps1 -Version 1.6.2 -SkipTests
```

### Méthode 2 : Manuellement

```bash
# 1. Mettre à jour la version dans Blazor.FlexLoader.csproj
# <Version>1.7.0</Version>

# 2. Commit et push
git add Blazor.FlexLoader.csproj
git commit -m "chore(release): bump version to 1.7.0"
git push origin develop

# 3. Merger dans main
git checkout main
git merge develop
git push origin main

# 4. Créer et pousser le tag
git tag v1.7.0
git push origin v1.7.0
```

---

## ?? Checklist de Vérification

Avant de publier, vérifier que :

- [ ] La version est mise à jour dans `Blazor.FlexLoader.csproj`
- [ ] Le `CHANGELOG.md` est à jour
- [ ] Les tests passent localement
- [ ] Le code est sur la branche `develop`
- [ ] `develop` est mergé dans `main`
- [ ] Le tag est au format `v*.*.*` (ex: `v1.7.0`)
- [ ] Le secret `NUGET_API_KEY` est configuré dans GitHub

---

## ?? Vérification Post-Publication

Après avoir poussé le tag, vérifier :

1. **GitHub Actions** :
   - https://github.com/daniwxcode/Blazor.FlexLoader/actions
   - Le workflow "?? Build and Publish NuGet Package" doit s'exécuter

2. **NuGet.org** (5-10 minutes de délai) :
   - https://www.nuget.org/packages/Blazor.FlexLoader

3. **GitHub Release** :
   - https://github.com/daniwxcode/Blazor.FlexLoader/releases

---

## ?? Dépannage

### Le workflow ne se déclenche pas

```bash
# Vérifier que le tag est bien poussé
git ls-remote --tags origin

# Si le tag n'est pas présent, le pousser
git push origin v1.7.0
```

### Erreur "NUGET_API_KEY is not set"

1. Aller dans **Settings ? Secrets and variables ? Actions**
2. Ajouter le secret `NUGET_API_KEY`
3. Relancer le workflow manuellement

### Le package n'apparaît pas sur NuGet.org

- Attendre 5-10 minutes (délai de propagation)
- Vérifier les logs du workflow pour détecter les erreurs
- Vérifier que la clé API est valide

---

## ?? Documentation

- [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md) - Guide complet de déploiement
- [.github/workflows/nuget-publish.yml](.github/workflows/nuget-publish.yml) - Workflow de publication
- [release.ps1](release.ps1) - Script de release automatisé

---

## ?? Prochaines Étapes pour Publier la v1.6.1 Correctement

Le tag `v1.6.1` existe déjà, mais le workflow peut ne pas avoir fonctionné correctement.

### Option 1 : Republier v1.6.1 (si pas encore sur NuGet.org)

```bash
# Déclencher manuellement le workflow
# Aller sur GitHub ? Actions ? ?? Build and Publish NuGet Package
# Cliquer sur "Run workflow" et sélectionner "main"
```

### Option 2 : Publier v1.6.2 (Patch correctif)

```powershell
.\release.ps1 -Version 1.6.2 -ReleaseNotes "Fix: Correction du workflow de déploiement automatique"
```

### Option 3 : Publier v1.7.0 (Version mineure)

```powershell
.\release.ps1 -Version 1.7.0 -ReleaseNotes "Fix: Correction du déploiement automatique + améliorations du workflow CI/CD"
```

---

**Date de correction** : 2024
**Auteur** : GitHub Copilot + daniwxcode
