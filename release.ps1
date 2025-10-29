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

# V�rifier le format de la version
if ($Version -notmatch '^\d+\.\d+\.\d+$') {
    Write-Error "? Format de version invalide. Utilisez le format X.Y.Z (ex: 1.7.0)"
    exit 1
}

$TagVersion = "v$Version"

Write-Info "========================================="
Write-Info "?? Publication de la version $Version"
Write-Info "========================================="

# 1. V�rifier qu'on est sur develop
Write-Info "`n1?? V�rification de la branche courante..."
$currentBranch = git branch --show-current
if ($currentBranch -ne "develop") {
    Write-Warning "??  Vous n'�tes pas sur la branche 'develop'"
    $continue = Read-Host "Voulez-vous continuer ? (o/N)"
    if ($continue -ne "o") {
        exit 0
    }
}
Write-Success "? Branche: $currentBranch"

# 2. V�rifier qu'il n'y a pas de changements non commit�s
Write-Info "`n2?? V�rification des changements non commit�s..."
$status = git status --porcelain
if ($status) {
Write-Error "? Vous avez des changements non commit�s:"
    git status --short
    exit 1
}
Write-Success "? Aucun changement non commit�"

# 3. Mettre � jour depuis le remote
Write-Info "`n3?? Mise � jour depuis le d�p�t distant..."
git fetch origin
git pull origin develop
Write-Success "? Branche � jour"

# 4. V�rifier que le tag n'existe pas d�j�
Write-Info "`n4?? V�rification du tag..."
$existingTag = git tag -l $TagVersion
if ($existingTag) {
    Write-Error "? Le tag $TagVersion existe d�j�"
    exit 1
}
Write-Success "? Tag $TagVersion disponible"

# 5. Mettre � jour la version dans le .csproj
Write-Info "`n5?? Mise � jour de la version dans Blazor.FlexLoader.csproj..."
$csprojPath = "Blazor.FlexLoader.csproj"
$csprojContent = Get-Content $csprojPath -Raw

# Mettre � jour la balise <Version>
$csprojContent = $csprojContent -replace '<Version>[\d\.]+</Version>', "<Version>$Version</Version>"

# Mettre � jour les PackageReleaseNotes si fourni
if ($ReleaseNotes) {
    $csprojContent = $csprojContent -replace '<PackageReleaseNotes>.*?</PackageReleaseNotes>', "<PackageReleaseNotes>v$Version`: $ReleaseNotes</PackageReleaseNotes>"
}

Set-Content $csprojPath -Value $csprojContent -NoNewline
Write-Success "? Version mise � jour: $Version"

# 6. Build et tests
if (-not $SkipTests) {
    Write-Info "`n6?? Build du projet..."
    dotnet restore
    dotnet build --configuration Release
    
    if ($LASTEXITCODE -ne 0) {
        Write-Error "? La build a �chou�"
        exit 1
    }
    Write-Success "? Build r�ussie"
    
    Write-Info "`n?? Ex�cution des tests..."
    $testProjects = Get-ChildItem -Path . -Recurse -Filter "*Tests.csproj"
    if ($testProjects) {
        dotnet test --configuration Release --no-build
      if ($LASTEXITCODE -ne 0) {
      Write-Error "? Les tests ont �chou�"
       exit 1
        }
 Write-Success "? Tests pass�s"
    } else {
        Write-Warning "??  Aucun projet de test trouv�"
  }
} else {
    Write-Warning "??  Tests ignor�s (--SkipTests)"
}

# 7. Cr�er un package de test
Write-Info "`n7?? Cr�ation du package NuGet..."
dotnet pack --configuration Release --output ./test-packages
if ($LASTEXITCODE -ne 0) {
    Write-Error "? La cr�ation du package a �chou�"
    exit 1
}
Write-Success "? Package cr�� dans ./test-packages/"

# 8. Afficher un r�sum�
Write-Info "`n========================================="
Write-Info "?? R�SUM� DE LA RELEASE"
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
    Write-Warning "`n??  MODE DRY RUN - Aucune modification ne sera commit�e"
    Write-Info "`nCommandes qui seraient ex�cut�es:"
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
    Write-Info "`n? Fichier .csproj restaur�"
    exit 0
}

# 10. Confirmer avant de continuer
Write-Warning "`n??  �tes-vous s�r de vouloir publier la version $Version ?"
$confirm = Read-Host "Taper 'PUBLISH' pour confirmer"
if ($confirm -ne "PUBLISH") {
    Write-Info "? Publication annul�e"
 git checkout $csprojPath
    exit 0
}

# 11. Commit et push
Write-Info "`n8?? Commit des changements..."
git add $csprojPath
git commit -m "chore(release): bump version to $Version"
git push origin develop
Write-Success "? Changements commit�s et pouss�s sur develop"

# 12. Merge vers main
Write-Info "`n9?? Merge vers main..."
git checkout main
git pull origin main
git merge develop --no-ff -m "chore(release): release version $Version"
git push origin main
Write-Success "? Changements merg�s dans main"

# 13. Cr�er et pousser le tag
Write-Info "`n?? Cr�ation et push du tag..."
git tag $TagVersion -m "Release version $Version"
git push origin $TagVersion
Write-Success "? Tag $TagVersion cr�� et pouss�"

# 14. Retourner sur develop
Write-Info "`n?? Retour sur develop..."
git checkout develop
Write-Success "? Retour sur la branche develop"

# 15. Afficher les instructions finales
Write-Success "`n========================================="
Write-Success "?? PUBLICATION R�USSIE !"
Write-Success "========================================="
Write-Info "Version      : $Version"
Write-Info "Tag          : $TagVersion"
Write-Info ""
Write-Info "?? Prochaines �tapes:"
Write-Info "1. V�rifier le workflow GitHub Actions:"
Write-Info "   https://github.com/daniwxcode/Blazor.FlexLoader/actions"
Write-Info ""
Write-Info "2. V�rifier la publication sur NuGet.org (peut prendre 5-10 min):"
Write-Info "https://www.nuget.org/packages/Blazor.FlexLoader/$Version"
Write-Info ""
Write-Info "3. V�rifier la GitHub Release:"
Write-Info "   https://github.com/daniwxcode/Blazor.FlexLoader/releases/tag/$TagVersion"
Write-Success "========================================="
