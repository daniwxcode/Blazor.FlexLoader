# ?? Guide de Choix de Version - Prochaine Publication

## ?? État Actuel

| Élément | Status | Détails |
|---------|--------|---------|
| **Version dans .csproj** | `1.6.1` | Déclarée dans Blazor.FlexLoader.csproj |
| **Dernier tag Git** | `v1.6.1` | Créé le 28 oct 2025 |
| **Branche develop** | En avance | 7 commits devant main |
| **Branche main** | En retard | Pas à jour avec develop |
| **NuGet.org** | ?? À vérifier | Vérifier si v1.6.1 est publié |

## ?? Vérifier si v1.6.1 est sur NuGet.org

```powershell
# Rechercher le package
dotnet nuget search Blazor.FlexLoader --exact-match

# Ou vérifier directement sur
# https://www.nuget.org/packages/Blazor.FlexLoader
```

## ?? Scénarios et Recommandations

### Scénario 1 : v1.6.1 N'EST PAS sur NuGet.org ? RECOMMANDÉ

**Recommandation** : Republier v1.6.1 avec les corrections

**Avantages** :
- ? Cohérence avec le tag existant
- ? Pas besoin de changer la version
- ? Les utilisateurs ayant v1.6.1 reçoivent la bonne version

**Actions** :

```bash
# Option A : Exécution manuelle du workflow
# 1. Aller sur GitHub ? Actions ? "?? Build and Publish NuGet Package"
# 2. Cliquer "Run workflow"
# 3. Sélectionner "main"
# 4. Laisser version vide (utilisera 1.6.1 du .csproj)
# 5. Cliquer "Run workflow"

# Option B : Merger develop dans main et re-pousser le tag
git checkout main
git pull origin main
git merge develop
git push origin main

# Le tag v1.6.1 existe déjà, le workflow devrait se déclencher
# Si non, déclencher manuellement (Option A)
```

---

### Scénario 2 : v1.6.1 EST sur NuGet.org mais incomplète

**Recommandation** : Publier v1.6.2 (patch)

**Raison** : On ne peut pas écraser une version existante sur NuGet.org

**Actions** :

```powershell
# Utiliser le script automatisé
.\release.ps1 -Version 1.6.2 -ReleaseNotes "Fix: Workflow de déploiement automatique + Publication des symboles"

# OU manuellement
# 1. Modifier Blazor.FlexLoader.csproj : <Version>1.6.2</Version>
# 2. git commit -am "chore(release): bump version to 1.6.2"
# 3. git checkout main && git merge develop && git push origin main
# 4. git tag v1.6.2 && git push origin v1.6.2
```

---

### Scénario 3 : Profiter pour une version mineure v1.7.0

**Recommandation** : Publier v1.7.0 si nouvelles fonctionnalités

**Raison** : Signaler les améliorations CI/CD et documentation

**Nouvelles fonctionnalités/améliorations** :
- ? Workflow de publication amélioré
- ? Support pour exécution manuelle
- ? Publication automatique des symboles (`.snupkg`)
- ? Vérification de la clé API
- ? Script de release automatisé
- ? Documentation complète du déploiement
- ? Meilleure gestion des artifacts

**Actions** :

```powershell
# Utiliser le script automatisé
.\release.ps1 -Version 1.7.0 -ReleaseNotes "Amélioration du système de publication automatique + Documentation complète"

# OU manuellement
# 1. Modifier Blazor.FlexLoader.csproj : <Version>1.7.0</Version>
# 2. Mettre à jour CHANGELOG.md
# 3. git commit -am "chore(release): bump version to 1.7.0"
# 4. git checkout main && git merge develop && git push origin main
# 5. git tag v1.7.0 && git push origin v1.7.0
```

---

## ?? Notre Recommandation

### Si v1.6.1 n'est PAS sur NuGet.org :
```powershell
# Republier v1.6.1 via exécution manuelle du workflow
# GitHub ? Actions ? "?? Build and Publish NuGet Package" ? Run workflow
```

### Si v1.6.1 EST sur NuGet.org :
```powershell
# Publier v1.6.2 (correction mineure)
.\release.ps1 -Version 1.6.2 -ReleaseNotes "Fix: Amélioration du workflow de publication et de la documentation"
```

### Pour une communication forte des améliorations :
```powershell
# Publier v1.7.0 (version mineure)
.\release.ps1 -Version 1.7.0 -ReleaseNotes "Amélioration: Système de publication automatique robuste + Documentation complète + Script de release"
```

---

## ?? Notes de Version Suggérées

### Pour v1.6.2 (Patch)

```
## [1.6.2] - 2024-10-29

### Fixed
- ?? Workflow de publication automatique sur NuGet.org
- ?? Publication automatique des symboles (.snupkg)
- ? Vérification de la clé API NuGet avant publication

### Added
- ?? Documentation complète du processus de déploiement
- ?? Script PowerShell pour automatiser les releases
- ?? Guide de dépannage pour les problèmes de publication

### Changed
- ?? Amélioration du workflow GitHub Actions
- ?? Support pour exécution manuelle du workflow
```

### Pour v1.7.0 (Mineure)

```
## [1.7.0] - 2024-10-29

### Added
- ?? Script de release automatisé (release.ps1)
- ?? Documentation complète de déploiement (DEPLOYMENT_GUIDE.md)
- ?? Guide de dépannage et correction (DEPLOYMENT_FIX.md)
- ? Vérification automatique de la clé API NuGet
- ?? Publication automatique des symboles de débogage (.snupkg)
- ?? Support pour exécution manuelle des workflows
- ?? Extraction intelligente de la version (tag/input/csproj)
- ?? Meilleure rétention des artifacts (30 jours)

### Fixed
- ?? Workflow de publication automatique sur NuGet.org
- ?? Conflit entre workflows de publication
- ?? Synchronisation entre branches develop et main

### Changed
- ?? Workflow `publish.yml` marqué comme DEPRECATED
- ?? Workflow `nuget-publish.yml` désormais principal
- ?? Amélioration des messages et logs des workflows
- ?? Documentation des workflows dans .github/workflows/README.md

### Documentation
- ? DEPLOYMENT_GUIDE.md - Guide complet de déploiement
- ? DEPLOYMENT_FIX.md - Détails de la correction
- ? DEPLOYMENT_SUMMARY.md - Résumé des corrections
- ? VERSION_CHOICE_GUIDE.md - Guide de choix de version
- ? .github/workflows/README.md - Documentation des workflows
```

---

## ? Quick Start - Publier Maintenant

### Vérification Préalable

```powershell
# 1. Vérifier si v1.6.1 existe sur NuGet
dotnet nuget search Blazor.FlexLoader --exact-match

# 2. Vérifier la branche actuelle
git branch --show-current

# 3. Vérifier qu'il n'y a pas de changements non commités
git status
```

### Publication Rapide

```powershell
# Si v1.6.1 n'existe PAS sur NuGet.org ? Republier v1.6.1
# GitHub ? Actions ? Run workflow manually

# Si v1.6.1 existe sur NuGet.org ? Publier v1.6.2
.\release.ps1 -Version 1.6.2

# Pour une version mineure ? Publier v1.7.0
.\release.ps1 -Version 1.7.0
```

### Test Sans Commit (Dry Run)

```powershell
# Tester le script sans faire de commit
.\release.ps1 -Version 1.7.0 -DryRun
```

---

## ?? Besoin d'Aide ?

Consultez les fichiers de documentation :

- **DEPLOYMENT_GUIDE.md** - Guide complet de déploiement
- **DEPLOYMENT_FIX.md** - Explication du problème et solutions
- **DEPLOYMENT_SUMMARY.md** - Résumé des corrections appliquées
- **.github/workflows/README.md** - Documentation des workflows

---

**Date** : 2024-10-29  
**Version du guide** : 1.0  
**Status** : ? Prêt pour décision et publication
