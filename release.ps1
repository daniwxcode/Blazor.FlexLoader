# ========================================
# ?? Blazor.FlexLoader - Release Script
# ========================================
# Ce script automatise le processus de publication d'une nouvelle version

param(
  [Parameter(Mandatory=$true)]
    [string]$Version,
    
    [Parameter(Mandatory=$false)]
    [string]$ReleaseNotes = "",
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipTests,
    
    [Parameter(Mandatory=$false)]
    [switch]$DryRun
)

$ErrorActionPreference = "Stop"

# Couleurs pour l'affichage
function Write-Success { Write-Host $args -ForegroundColor Green }
function Write-Info { Write-Host $args -ForegroundColor Cyan }
function Write-Warning { Write-Host $args -ForegroundColor Yellow }
function Write-Error { Write-Host $args -ForegroundColor Red }

# Vérifier le format de la version
if ($Version -notmatch '^\d+\.\d+\.\d+$') {
    Write-Error "? Format de version invalide. Utilisez le format X.Y.Z (ex: 1.7.0)"
    exit 1
}

$TagVersion = "v$Version"

Write-Info "========================================="
Write-Info "?? Publication de la version $Version"
Write-Info "========================================="

# 1. Vérifier qu'on est sur develop
Write-Info "`n1?? Vérification de la branche courante..."
$currentBranch = git branch --show-current
if ($currentBranch -ne "develop") {
    Write-Warning "??  Vous n'êtes pas sur la branche 'develop'"
    $continue = Read-Host "Voulez-vous continuer ? (o/N)"
    if ($continue -ne "o") {
        exit 0
    }
}
Write-Success "? Branche: $currentBranch"

# 2. Vérifier qu'il n'y a pas de changements non commités
Write-Info "`n2?? Vérification des changements non commités..."
$status = git status --porcelain
if ($status) {
Write-Error "? Vous avez des changements non commités:"
    git status --short
    exit 1
}
Write-Success "? Aucun changement non commité"

# 3. Mettre à jour depuis le remote
Write-Info "`n3?? Mise à jour depuis le dépôt distant..."
git fetch origin
git pull origin develop
Write-Success "? Branche à jour"

# 4. Vérifier que le tag n'existe pas déjà
Write-Info "`n4?? Vérification du tag..."
$existingTag = git tag -l $TagVersion
if ($existingTag) {
    Write-Error "? Le tag $TagVersion existe déjà"
    exit 1
}
Write-Success "? Tag $TagVersion disponible"

# 5. Mettre à jour la version dans le .csproj
Write-Info "`n5?? Mise à jour de la version dans Blazor.FlexLoader.csproj..."
$csprojPath = "Blazor.FlexLoader.csproj"
$csprojContent = Get-Content $csprojPath -Raw

# Mettre à jour la balise <Version>
$csprojContent = $csprojContent -replace '<Version>[\d\.]+</Version>', "<Version>$Version</Version>"

# Mettre à jour les PackageReleaseNotes si fourni
if ($ReleaseNotes) {
    $csprojContent = $csprojContent -replace '<PackageReleaseNotes>.*?</PackageReleaseNotes>', "<PackageReleaseNotes>v$Version`: $ReleaseNotes</PackageReleaseNotes>"
}

Set-Content $csprojPath -Value $csprojContent -NoNewline
Write-Success "? Version mise à jour: $Version"

# 6. Build et tests
if (-not $SkipTests) {
    Write-Info "`n6?? Build du projet..."
    dotnet restore
    dotnet build --configuration Release
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "? La build a échoué"
        exit 1
    }
    Write-Success "? Build réussie"
    
    Write-Info "`n?? Exécution des tests..."
    $testProjects = Get-ChildItem -Path . -Recurse -Filter "*Tests.csproj"
    if ($testProjects) {
        dotnet test --configuration Release --no-build
      if ($LASTEXITCODE -ne 0) {
      Write-Error "? Les tests ont échoué"
       exit 1
        }
 Write-Success "? Tests passés"
    } else {
        Write-Warning "??  Aucun projet de test trouvé"
  }
} else {
    Write-Warning "??  Tests ignorés (--SkipTests)"
}

# 7. Créer un package de test
Write-Info "`n7?? Création du package NuGet..."
dotnet pack --configuration Release --output ./test-packages
if ($LASTEXITCODE -ne 0) {
    Write-Error "? La création du package a échoué"
    exit 1
}
Write-Success "? Package créé dans ./test-packages/"

# 8. Afficher un résumé
Write-Info "`n========================================="
Write-Info "?? RÉSUMÉ DE LA RELEASE"
Write-Info "========================================="
Write-Info "Version      : $Version"
Write-Info "Tag          : $TagVersion"
Write-Info "Branche      : $currentBranch"
if ($ReleaseNotes) {
    Write-Info "Notes : $ReleaseNotes"
}
Write-Info "========================================="

# 9. Mode Dry Run
if ($DryRun) {
    Write-Warning "`n??  MODE DRY RUN - Aucune modification ne sera commitée"
    Write-Info "`nCommandes qui seraient exécutées:"
    Write-Info "  git add Blazor.FlexLoader.csproj"
    Write-Info "  git commit -m 'chore(release): bump version to $Version'"
    Write-Info "  git push origin develop"
    Write-Info "  git checkout main"
    Write-Info "  git merge develop"
    Write-Info "  git push origin main"
    Write-Info "  git tag $TagVersion"
    Write-Info "  git push origin $TagVersion"

    # Restaurer le fichier .csproj
    git checkout $csprojPath
    Write-Info "`n? Fichier .csproj restauré"
    exit 0
}

# 10. Confirmer avant de continuer
Write-Warning "`n??  Êtes-vous sûr de vouloir publier la version $Version ?"
$confirm = Read-Host "Taper 'PUBLISH' pour confirmer"
if ($confirm -ne "PUBLISH") {
    Write-Info "? Publication annulée"
 git checkout $csprojPath
    exit 0
}

# 11. Commit et push
Write-Info "`n8?? Commit des changements..."
git add $csprojPath
git commit -m "chore(release): bump version to $Version"
git push origin develop
Write-Success "? Changements commités et poussés sur develop"

# 12. Merge vers main
Write-Info "`n9?? Merge vers main..."
git checkout main
git pull origin main
git merge develop --no-ff -m "chore(release): release version $Version"
git push origin main
Write-Success "? Changements mergés dans main"

# 13. Créer et pousser le tag
Write-Info "`n?? Création et push du tag..."
git tag $TagVersion -m "Release version $Version"
git push origin $TagVersion
Write-Success "? Tag $TagVersion créé et poussé"

# 14. Retourner sur develop
Write-Info "`n?? Retour sur develop..."
git checkout develop
Write-Success "? Retour sur la branche develop"

# 15. Afficher les instructions finales
Write-Success "`n========================================="
Write-Success "?? PUBLICATION RÉUSSIE !"
Write-Success "========================================="
Write-Info "Version      : $Version"
Write-Info "Tag          : $TagVersion"
Write-Info ""
Write-Info "?? Prochaines étapes:"
Write-Info "1. Vérifier le workflow GitHub Actions:"
Write-Info "   https://github.com/daniwxcode/Blazor.FlexLoader/actions"
Write-Info ""
Write-Info "2. Vérifier la publication sur NuGet.org (peut prendre 5-10 min):"
Write-Info "https://www.nuget.org/packages/Blazor.FlexLoader/$Version"
Write-Info ""
Write-Info "3. Vérifier la GitHub Release:"
Write-Info "   https://github.com/daniwxcode/Blazor.FlexLoader/releases/tag/$TagVersion"
Write-Success "========================================="
