# ?? Correction du Probl�me de D�ploiement Automatique

## ?? Probl�me Identifi�

Depuis la version 1.4, le package NuGet ne se d�ploie plus automatiquement sur NuGet.org.

### Causes Identifi�es

1. **Conflit entre deux workflows** :
   - `publish.yml` : Se d�clenche automatiquement sur chaque push vers `main`
   - `nuget-publish.yml` : Se d�clenche uniquement sur les tags `v*.*.*`

2. **Changements sur la branche `develop` non merg�s dans `main`** :
   - Les derniers commits sont sur `develop`
   - La branche `main` est en retard
   - Le tag `v1.6.1` existe, mais le code n'est pas sur `main`

3. **Workflow bas� sur les tags non d�clench� correctement**

---

## ? Solutions Appliqu�es

### 1. D�sactivation du workflow `publish.yml`

Le workflow `publish.yml` a �t� d�sactiv� pour �viter les conflits et publications non d�sir�es.

**Raison** : Publication automatique sur chaque modification de `main` n'est pas recommand�e.

### 2. Am�lioration du workflow `nuget-publish.yml`

- ? Support pour l'ex�cution manuelle (`workflow_dispatch`)
- ? V�rification de la cl� API NuGet avant publication
- ? Publication des symboles (`.snupkg`)
- ? Meilleure gestion des erreurs
- ? Logs plus d�taill�s avec emojis

### 3. Cr�ation d'un guide de d�ploiement

Un guide complet (`DEPLOYMENT_GUIDE.md`) a �t� cr�� avec :
- Processus de publication recommand�
- Configuration requise
- Checklist de publication
- Section de d�pannage

### 4. Script PowerShell de release automatis�

Un script `release.ps1` a �t� cr�� pour automatiser :
- V�rification de la branche et des changements
- Mise � jour de la version dans `.csproj`
- Build et tests
- Cr�ation du package
- Commit, merge et push
- Cr�ation et push du tag

---

## ?? Processus de Publication Recommand�

### M�thode 1 : Utiliser le script PowerShell (Recommand�)

```powershell
# Publication d'une nouvelle version
.\release.ps1 -Version 1.7.0 -ReleaseNotes "Correction du d�ploiement automatique"

# Dry run (test sans commit)
.\release.ps1 -Version 1.7.0 -DryRun

# Sans tests (plus rapide pour les hotfixes)
.\release.ps1 -Version 1.6.2 -SkipTests
```

### M�thode 2 : Manuellement

```bash
# 1. Mettre � jour la version dans Blazor.FlexLoader.csproj
# <Version>1.7.0</Version>

# 2. Commit et push
git add Blazor.FlexLoader.csproj
git commit -m "chore(release): bump version to 1.7.0"
git push origin develop

# 3. Merger dans main
git checkout main
git merge develop
git push origin main

# 4. Cr�er et pousser le tag
git tag v1.7.0
git push origin v1.7.0
```

---

## ?? Checklist de V�rification

Avant de publier, v�rifier que :

- [ ] La version est mise � jour dans `Blazor.FlexLoader.csproj`
- [ ] Le `CHANGELOG.md` est � jour
- [ ] Les tests passent localement
- [ ] Le code est sur la branche `develop`
- [ ] `develop` est merg� dans `main`
- [ ] Le tag est au format `v*.*.*` (ex: `v1.7.0`)
- [ ] Le secret `NUGET_API_KEY` est configur� dans GitHub

---

## ?? V�rification Post-Publication

Apr�s avoir pouss� le tag, v�rifier :

1. **GitHub Actions** :
   - https://github.com/daniwxcode/Blazor.FlexLoader/actions
   - Le workflow "?? Build and Publish NuGet Package" doit s'ex�cuter

2. **NuGet.org** (5-10 minutes de d�lai) :
   - https://www.nuget.org/packages/Blazor.FlexLoader

3. **GitHub Release** :
   - https://github.com/daniwxcode/Blazor.FlexLoader/releases

---

## ?? D�pannage

### Le workflow ne se d�clenche pas

```bash
# V�rifier que le tag est bien pouss�
git ls-remote --tags origin

# Si le tag n'est pas pr�sent, le pousser
git push origin v1.7.0
```

### Erreur "NUGET_API_KEY is not set"

1. Aller dans **Settings ? Secrets and variables ? Actions**
2. Ajouter le secret `NUGET_API_KEY`
3. Relancer le workflow manuellement

### Le package n'appara�t pas sur NuGet.org

- Attendre 5-10 minutes (d�lai de propagation)
- V�rifier les logs du workflow pour d�tecter les erreurs
- V�rifier que la cl� API est valide

---

## ?? Documentation

- [DEPLOYMENT_GUIDE.md](DEPLOYMENT_GUIDE.md) - Guide complet de d�ploiement
- [.github/workflows/nuget-publish.yml](.github/workflows/nuget-publish.yml) - Workflow de publication
- [release.ps1](release.ps1) - Script de release automatis�

---

## ?? Prochaines �tapes pour Publier la v1.6.1 Correctement

Le tag `v1.6.1` existe d�j�, mais le workflow peut ne pas avoir fonctionn� correctement.

### Option 1 : Republier v1.6.1 (si pas encore sur NuGet.org)

```bash
# D�clencher manuellement le workflow
# Aller sur GitHub ? Actions ? ?? Build and Publish NuGet Package
# Cliquer sur "Run workflow" et s�lectionner "main"
```

### Option 2 : Publier v1.6.2 (Patch correctif)

```powershell
.\release.ps1 -Version 1.6.2 -ReleaseNotes "Fix: Correction du workflow de d�ploiement automatique"
```

### Option 3 : Publier v1.7.0 (Version mineure)

```powershell
.\release.ps1 -Version 1.7.0 -ReleaseNotes "Fix: Correction du d�ploiement automatique + am�liorations du workflow CI/CD"
```

---

**Date de correction** : 2024
**Auteur** : GitHub Copilot + daniwxcode
