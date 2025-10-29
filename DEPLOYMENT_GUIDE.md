# ?? Guide de Déploiement NuGet - Blazor.FlexLoader

## ?? Processus de Publication Recommandé

### 1?? Préparer la Release

1. **Mettre à jour la version dans `Blazor.FlexLoader.csproj`**
   ```xml
   <Version>1.7.0</Version>
   ```

2. **Mettre à jour le `CHANGELOG.md`**
   ```markdown
   ## [1.7.0] - 2024-XX-XX
   ### Added
   - Nouvelle fonctionnalité
   ### Fixed
   - Correction de bug
   ```

3. **Mettre à jour les `PackageReleaseNotes`** dans le `.csproj`

4. **Commit et push vers `develop`**
   ```bash
   git add .
   git commit -m "chore(release): bump version to 1.7.0"
   git push origin develop
   ```

### 2?? Merger vers `main`

```bash
git checkout main
git merge develop
git push origin main
```

### 3?? Créer et Pousser le Tag

**Important**: Le workflow de publication se déclenche uniquement sur les tags au format `v*.*.*`

```bash
# Créer le tag localement
git tag v1.7.0

# Pousser le tag vers GitHub
git push origin v1.7.0
```

### 4?? Vérifier le Workflow

1. Aller sur GitHub ? Actions
2. Vérifier que le workflow **"?? Build and Publish NuGet Package"** s'exécute
3. Vérifier les logs pour détecter d'éventuelles erreurs

---

## ?? Configuration Requise

### Secrets GitHub à Configurer

Dans **Settings ? Secrets and variables ? Actions**, ajouter :

| Secret | Description |
|--------|-------------|
| `NUGET_API_KEY` | Clé API de NuGet.org |

**Comment obtenir une clé API NuGet** :
1. Se connecter sur [NuGet.org](https://www.nuget.org)
2. Aller dans **Account Settings ? API Keys**
3. Créer une nouvelle clé avec les permissions :
   - **Push** (pour publier)
   - **Unlist** (optionnel)
   - **Scope**: Sélectionner le package `Blazor.FlexLoader`

---

## ??? Publication Manuelle (Fallback)

Si le workflow automatique échoue, vous pouvez publier manuellement :

### Via GitHub Actions (Interface Web)

1. Aller sur **Actions ? ?? Build and Publish NuGet Package**
2. Cliquer sur **Run workflow**
3. Sélectionner la branche `main`
4. (Optionnel) Spécifier la version
5. Cliquer sur **Run workflow**

### Via Ligne de Commande

```bash
# 1. Build
dotnet build Blazor.FlexLoader.csproj --configuration Release

# 2. Pack
dotnet pack Blazor.FlexLoader.csproj --configuration Release --output ./artifacts

# 3. Publish
dotnet nuget push "./artifacts/*.nupkg" \
  --api-key YOUR_NUGET_API_KEY \
  --source https://api.nuget.org/v3/index.json \
  --skip-duplicate
```

---

## ?? Checklist de Publication

- [ ] Version mise à jour dans `.csproj`
- [ ] `CHANGELOG.md` mis à jour
- [ ] `PackageReleaseNotes` mis à jour
- [ ] Code committé et pushé sur `develop`
- [ ] `develop` mergé dans `main`
- [ ] Tag `v*.*.*` créé et poussé
- [ ] Workflow GitHub Actions vérifié
- [ ] Package visible sur [NuGet.org](https://www.nuget.org/packages/Blazor.FlexLoader)
- [ ] GitHub Release créée automatiquement

---

## ?? Dépannage

### Le workflow ne se déclenche pas

**Causes possibles** :
1. ? Tag mal formaté (doit être `v1.7.0`, pas `1.7.0`)
2. ? Tag non poussé vers GitHub (`git push origin v1.7.0`)
3. ? Workflow désactivé dans Settings ? Actions

**Solution** :
```bash
# Vérifier les tags locaux
git tag -l

# Vérifier les tags distants
git ls-remote --tags origin

# Supprimer un tag mal formaté
git tag -d 1.7.0
git push origin :refs/tags/1.7.0

# Créer et pousser le bon tag
git tag v1.7.0
git push origin v1.7.0
```

### Erreur "NUGET_API_KEY is not set"

**Solution** :
1. Aller dans **Settings ? Secrets and variables ? Actions**
2. Ajouter le secret `NUGET_API_KEY`
3. Relancer le workflow

### Erreur "Package already exists"

**Solution** :
- C'est normal si la version existe déjà
- Le flag `--skip-duplicate` ignore cette erreur
- Incrémenter la version dans `.csproj` et créer un nouveau tag

### Le package n'apparaît pas sur NuGet.org

**Causes possibles** :
1. ? Délai de propagation (peut prendre 5-10 minutes)
2. ? Erreur lors de la publication (vérifier les logs du workflow)
3. ? Clé API invalide ou expirée

**Solution** :
```bash
# Vérifier si le package existe
dotnet nuget search Blazor.FlexLoader --exact-match
```

---

## ?? Workflows Disponibles

### 1. `nuget-publish.yml` (PRINCIPAL) ?

**Déclencheur** : Tags `v*.*.*` ou exécution manuelle

**Utilisation** :
```bash
git tag v1.7.0
git push origin v1.7.0
```

### 2. `publish.yml` (DÉPRÉCIÉ) ??

**Statut** : Désactivé pour éviter les conflits

**Raison** : Publication automatique sur chaque push vers `main` causait des publications non désirées

### 3. `ci.yml` (VALIDATION)

**Déclencheur** : Push ou PR vers `main` ou `develop`

**Objectif** : Valider le code sans publier

---

## ?? Bonnes Pratiques

1. **Toujours créer une PR de `develop` vers `main`** pour review
2. **Tester localement** avant de créer le tag
3. **Suivre le versioning sémantique** (SemVer) :
   - `MAJOR.MINOR.PATCH` (ex: `1.7.0`)
   - `MAJOR` : Breaking changes
   - `MINOR` : Nouvelles fonctionnalités
   - `PATCH` : Corrections de bugs
4. **Ne jamais supprimer un tag publié** sur NuGet (impossible d'unlist)
5. **Garder `CHANGELOG.md` à jour** pour chaque version

---

## ?? Ressources

- [NuGet.org - Blazor.FlexLoader](https://www.nuget.org/packages/Blazor.FlexLoader)
- [GitHub Repository](https://github.com/daniwxcode/Blazor.FlexLoader)
- [Documentation](https://github.com/daniwxcode/Blazor.FlexLoader#readme)
- [Changelog](CHANGELOG.md)

---

**Dernière mise à jour** : 2024
**Version du guide** : 1.0
