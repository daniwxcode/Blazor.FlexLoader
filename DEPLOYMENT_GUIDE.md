# ?? Guide de D�ploiement NuGet - Blazor.FlexLoader

## ?? Processus de Publication Recommand�

### 1?? Pr�parer la Release

1. **Mettre � jour la version dans `Blazor.FlexLoader.csproj`**
   ```xml
   <Version>1.7.0</Version>
   ```

2. **Mettre � jour le `CHANGELOG.md`**
   ```markdown
   ## [1.7.0] - 2024-XX-XX
   ### Added
   - Nouvelle fonctionnalit�
   ### Fixed
   - Correction de bug
   ```

3. **Mettre � jour les `PackageReleaseNotes`** dans le `.csproj`

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

### 3?? Cr�er et Pousser le Tag

**Important**: Le workflow de publication se d�clenche uniquement sur les tags au format `v*.*.*`

```bash
# Cr�er le tag localement
git tag v1.7.0

# Pousser le tag vers GitHub
git push origin v1.7.0
```

### 4?? V�rifier le Workflow

1. Aller sur GitHub ? Actions
2. V�rifier que le workflow **"?? Build and Publish NuGet Package"** s'ex�cute
3. V�rifier les logs pour d�tecter d'�ventuelles erreurs

---

## ?? Configuration Requise

### Secrets GitHub � Configurer

Dans **Settings ? Secrets and variables ? Actions**, ajouter :

| Secret | Description |
|--------|-------------|
| `NUGET_API_KEY` | Cl� API de NuGet.org |

**Comment obtenir une cl� API NuGet** :
1. Se connecter sur [NuGet.org](https://www.nuget.org)
2. Aller dans **Account Settings ? API Keys**
3. Cr�er une nouvelle cl� avec les permissions :
   - **Push** (pour publier)
   - **Unlist** (optionnel)
   - **Scope**: S�lectionner le package `Blazor.FlexLoader`

---

## ??? Publication Manuelle (Fallback)

Si le workflow automatique �choue, vous pouvez publier manuellement :

### Via GitHub Actions (Interface Web)

1. Aller sur **Actions ? ?? Build and Publish NuGet Package**
2. Cliquer sur **Run workflow**
3. S�lectionner la branche `main`
4. (Optionnel) Sp�cifier la version
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

- [ ] Version mise � jour dans `.csproj`
- [ ] `CHANGELOG.md` mis � jour
- [ ] `PackageReleaseNotes` mis � jour
- [ ] Code committ� et push� sur `develop`
- [ ] `develop` merg� dans `main`
- [ ] Tag `v*.*.*` cr�� et pouss�
- [ ] Workflow GitHub Actions v�rifi�
- [ ] Package visible sur [NuGet.org](https://www.nuget.org/packages/Blazor.FlexLoader)
- [ ] GitHub Release cr��e automatiquement

---

## ?? D�pannage

### Le workflow ne se d�clenche pas

**Causes possibles** :
1. ? Tag mal format� (doit �tre `v1.7.0`, pas `1.7.0`)
2. ? Tag non pouss� vers GitHub (`git push origin v1.7.0`)
3. ? Workflow d�sactiv� dans Settings ? Actions

**Solution** :
```bash
# V�rifier les tags locaux
git tag -l

# V�rifier les tags distants
git ls-remote --tags origin

# Supprimer un tag mal format�
git tag -d 1.7.0
git push origin :refs/tags/1.7.0

# Cr�er et pousser le bon tag
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
- C'est normal si la version existe d�j�
- Le flag `--skip-duplicate` ignore cette erreur
- Incr�menter la version dans `.csproj` et cr�er un nouveau tag

### Le package n'appara�t pas sur NuGet.org

**Causes possibles** :
1. ? D�lai de propagation (peut prendre 5-10 minutes)
2. ? Erreur lors de la publication (v�rifier les logs du workflow)
3. ? Cl� API invalide ou expir�e

**Solution** :
```bash
# V�rifier si le package existe
dotnet nuget search Blazor.FlexLoader --exact-match
```

---

## ?? Workflows Disponibles

### 1. `nuget-publish.yml` (PRINCIPAL) ?

**D�clencheur** : Tags `v*.*.*` ou ex�cution manuelle

**Utilisation** :
```bash
git tag v1.7.0
git push origin v1.7.0
```

### 2. `publish.yml` (D�PR�CI�) ??

**Statut** : D�sactiv� pour �viter les conflits

**Raison** : Publication automatique sur chaque push vers `main` causait des publications non d�sir�es

### 3. `ci.yml` (VALIDATION)

**D�clencheur** : Push ou PR vers `main` ou `develop`

**Objectif** : Valider le code sans publier

---

## ?? Bonnes Pratiques

1. **Toujours cr�er une PR de `develop` vers `main`** pour review
2. **Tester localement** avant de cr�er le tag
3. **Suivre le versioning s�mantique** (SemVer) :
   - `MAJOR.MINOR.PATCH` (ex: `1.7.0`)
   - `MAJOR` : Breaking changes
   - `MINOR` : Nouvelles fonctionnalit�s
   - `PATCH` : Corrections de bugs
4. **Ne jamais supprimer un tag publi�** sur NuGet (impossible d'unlist)
5. **Garder `CHANGELOG.md` � jour** pour chaque version

---

## ?? Ressources

- [NuGet.org - Blazor.FlexLoader](https://www.nuget.org/packages/Blazor.FlexLoader)
- [GitHub Repository](https://github.com/daniwxcode/Blazor.FlexLoader)
- [Documentation](https://github.com/daniwxcode/Blazor.FlexLoader#readme)
- [Changelog](CHANGELOG.md)

---

**Derni�re mise � jour** : 2024
**Version du guide** : 1.0
