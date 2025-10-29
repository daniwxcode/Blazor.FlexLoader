# Deployment Guide - Blazor.FlexLoader

## Quick Publish

### Using PowerShell Script (Recommended)

```powershell
# Publish new version
.\release.ps1 -Version 1.7.0 -ReleaseNotes "New features"

# Test without commit
.\release.ps1 -Version 1.7.0 -DryRun
```

### Manual Process

```bash
# 1. Update version in Blazor.FlexLoader.csproj
# <Version>1.7.0</Version>

# 2. Commit and push
git add Blazor.FlexLoader.csproj
git commit -m "chore(release): bump version to 1.7.0"
git push origin develop

# 3. Merge to main
git checkout main
git merge develop
git push origin main

# 4. Create and push tag
git tag v1.7.0
git push origin v1.7.0
```

## GitHub Secrets Required

- `NUGET_API_KEY` - Get from https://nuget.org (Account Settings ? API Keys)

## Verification

1. GitHub Actions: https://github.com/daniwxcode/Blazor.FlexLoader/actions
2. NuGet Package: https://www.nuget.org/packages/Blazor.FlexLoader
3. GitHub Release: https://github.com/daniwxcode/Blazor.FlexLoader/releases

## Workflows

- **nuget-publish.yml** - Main workflow (triggered by tags v*.*.*)
- **ci.yml** - Validation on push/PR
- **publish.yml** - DEPRECATED (disabled)

## Troubleshooting

### Workflow not triggered
```bash
git ls-remote --tags origin
git push origin v1.7.0
```

### NUGET_API_KEY error
- Add secret in: Settings ? Secrets and variables ? Actions

### Package not on NuGet.org
- Wait 5-10 minutes
- Check workflow logs
- Verify API key validity
