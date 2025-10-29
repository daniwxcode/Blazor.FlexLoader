# ?? Guide de Choix de Version - Prochaine Publication

## ?? �tat Actuel

| �l�ment | Status | D�tails |
|---------|--------|---------|
| **Version dans .csproj** | `1.6.1` | D�clar�e dans Blazor.FlexLoader.csproj |
| **Dernier tag Git** | `v1.6.1` | Cr�� le 28 oct 2025 |
| **Branche develop** | En avance | 7 commits devant main |
| **Branche main** | En retard | Pas � jour avec develop |
| **NuGet.org** | ?? � v�rifier | V�rifier si v1.6.1 est publi� |

## ?? V�rifier si v1.6.1 est sur NuGet.org

```powershell
# Rechercher le package
dotnet nuget search Blazor.FlexLoader --exact-match

# Ou v�rifier directement sur
# https://www.nuget.org/packages/Blazor.FlexLoader
```

## ?? Sc�narios et Recommandations

### Sc�nario 1 : v1.6.1 N'EST PAS sur NuGet.org ? RECOMMAND�

**Recommandation** : Republier v1.6.1 avec les corrections

**Avantages** :
- ? Coh�rence avec le tag existant
- ? Pas besoin de changer la version
- ? Les utilisateurs ayant v1.6.1 re�oivent la bonne version

**Actions** :

```bash
# Option A : Ex�cution manuelle du workflow
# 1. Aller sur GitHub ? Actions ? "?? Build and Publish NuGet Package"
# 2. Cliquer "Run workflow"
# 3. S�lectionner "main"
# 4. Laisser version vide (utilisera 1.6.1 du .csproj)
# 5. Cliquer "Run workflow"

# Option B : Merger develop dans main et re-pousser le tag
git checkout main
git pull origin main
git merge develop
git push origin main

# Le tag v1.6.1 existe d�j�, le workflow devrait se d�clencher
# Si non, d�clencher manuellement (Option A)
```

---

### Sc�nario 2 : v1.6.1 EST sur NuGet.org mais incompl�te

**Recommandation** : Publier v1.6.2 (patch)

**Raison** : On ne peut pas �craser une version existante sur NuGet.org

**Actions** :

```powershell
# Utiliser le script automatis�
.\release.ps1 -Version 1.6.2 -ReleaseNotes "Fix: Workflow de d�ploiement automatique + Publication des symboles"

# OU manuellement
# 1. Modifier Blazor.FlexLoader.csproj : <Version>1.6.2</Version>
# 2. git commit -am "chore(release): bump version to 1.6.2"
# 3. git checkout main && git merge develop && git push origin main
# 4. git tag v1.6.2 && git push origin v1.6.2
```

---

### Sc�nario 3 : Profiter pour une version mineure v1.7.0

**Recommandation** : Publier v1.7.0 si nouvelles fonctionnalit�s

**Raison** : Signaler les am�liorations CI/CD et documentation

**Nouvelles fonctionnalit�s/am�liorations** :
- ? Workflow de publication am�lior�
- ? Support pour ex�cution manuelle
- ? Publication automatique des symboles (`.snupkg`)
- ? V�rification de la cl� API
- ? Script de release automatis�
- ? Documentation compl�te du d�ploiement
- ? Meilleure gestion des artifacts

**Actions** :

```powershell
# Utiliser le script automatis�
.\release.ps1 -Version 1.7.0 -ReleaseNotes "Am�lioration du syst�me de publication automatique + Documentation compl�te"

# OU manuellement
# 1. Modifier Blazor.FlexLoader.csproj : <Version>1.7.0</Version>
# 2. Mettre � jour CHANGELOG.md
# 3. git commit -am "chore(release): bump version to 1.7.0"
# 4. git checkout main && git merge develop && git push origin main
# 5. git tag v1.7.0 && git push origin v1.7.0
```

---

## ?? Notre Recommandation

### Si v1.6.1 n'est PAS sur NuGet.org :
```powershell
# Republier v1.6.1 via ex�cution manuelle du workflow
# GitHub ? Actions ? "?? Build and Publish NuGet Package" ? Run workflow
```

### Si v1.6.1 EST sur NuGet.org :
```powershell
# Publier v1.6.2 (correction mineure)
.\release.ps1 -Version 1.6.2 -ReleaseNotes "Fix: Am�lioration du workflow de publication et de la documentation"
```

### Pour une communication forte des am�liorations :
```powershell
# Publier v1.7.0 (version mineure)
.\release.ps1 -Version 1.7.0 -ReleaseNotes "Am�lioration: Syst�me de publication automatique robuste + Documentation compl�te + Script de release"
```

---

## ?? Notes de Version Sugg�r�es

### Pour v1.6.2 (Patch)

```
## [1.6.2] - 2024-10-29

### Fixed
- ?? Workflow de publication automatique sur NuGet.org
- ?? Publication automatique des symboles (.snupkg)
- ? V�rification de la cl� API NuGet avant publication

### Added
- ?? Documentation compl�te du processus de d�ploiement
- ?? Script PowerShell pour automatiser les releases
- ?? Guide de d�pannage pour les probl�mes de publication

### Changed
- ?? Am�lioration du workflow GitHub Actions
- ?? Support pour ex�cution manuelle du workflow
```

### Pour v1.7.0 (Mineure)

```
## [1.7.0] - 2024-10-29

### Added
- ?? Script de release automatis� (release.ps1)
- ?? Documentation compl�te de d�ploiement (DEPLOYMENT_GUIDE.md)
- ?? Guide de d�pannage et correction (DEPLOYMENT_FIX.md)
- ? V�rification automatique de la cl� API NuGet
- ?? Publication automatique des symboles de d�bogage (.snupkg)
- ?? Support pour ex�cution manuelle des workflows
- ?? Extraction intelligente de la version (tag/input/csproj)
- ?? Meilleure r�tention des artifacts (30 jours)

### Fixed
- ?? Workflow de publication automatique sur NuGet.org
- ?? Conflit entre workflows de publication
- ?? Synchronisation entre branches develop et main

### Changed
- ?? Workflow `publish.yml` marqu� comme DEPRECATED
- ?? Workflow `nuget-publish.yml` d�sormais principal
- ?? Am�lioration des messages et logs des workflows
- ?? Documentation des workflows dans .github/workflows/README.md

### Documentation
- ? DEPLOYMENT_GUIDE.md - Guide complet de d�ploiement
- ? DEPLOYMENT_FIX.md - D�tails de la correction
- ? DEPLOYMENT_SUMMARY.md - R�sum� des corrections
- ? VERSION_CHOICE_GUIDE.md - Guide de choix de version
- ? .github/workflows/README.md - Documentation des workflows
```

---

## ? Quick Start - Publier Maintenant

### V�rification Pr�alable

```powershell
# 1. V�rifier si v1.6.1 existe sur NuGet
dotnet nuget search Blazor.FlexLoader --exact-match

# 2. V�rifier la branche actuelle
git branch --show-current

# 3. V�rifier qu'il n'y a pas de changements non commit�s
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

- **DEPLOYMENT_GUIDE.md** - Guide complet de d�ploiement
- **DEPLOYMENT_FIX.md** - Explication du probl�me et solutions
- **DEPLOYMENT_SUMMARY.md** - R�sum� des corrections appliqu�es
- **.github/workflows/README.md** - Documentation des workflows

---

**Date** : 2024-10-29  
**Version du guide** : 1.0  
**Status** : ? Pr�t pour d�cision et publication
