# EasyBluetooth AIDA64 Helper

Open-source companion tool for sending EasyBluetooth battery data to AIDA64 `ImportValues`.

## What It Does

- Polls the local EasyBluetooth Unified Standard Data API
- Writes up to 10 device slots into `HKCU\Software\FinalWire\AIDA64\ImportValues`
- Helps SensorPanel and other AIDA64 skins display EasyBluetooth battery status

## Requirements

- Windows
- EasyBluetooth with Unified Standard Data API enabled
- VIP active if your EasyBluetooth setup requires the API entitlement
- AIDA64 installed if you want to see the imported values rendered there

## Project Layout

- Source project: `src/EasyBluetooth.Aida64Helper/EasyBluetooth.Aida64Helper.csproj`
- GitHub release workflow: `.github/workflows/release-aida64helper.yml`

## Local Build

```powershell
dotnet build .\tools\Aida64Helper\src\EasyBluetooth.Aida64Helper\EasyBluetooth.Aida64Helper.csproj -c Debug
```

## Local Publish

```powershell
dotnet publish .\tools\Aida64Helper\src\EasyBluetooth.Aida64Helper\EasyBluetooth.Aida64Helper.csproj `
  -c Release `
  -f net8.0-windows `
  -r win-x64 `
  --self-contained true `
  -p:PublishSingleFile=true `
  -p:IncludeNativeLibrariesForSelfExtract=true `
  -p:PublishReadyToRun=false
```

## GitHub Actions Usage

### Validate With Manual Run

1. Open the repository `Actions` tab.
2. Select `Release AIDA64 Helper`.
3. Click `Run workflow`.
4. Leave `release_tag` empty if you only want a build artifact.
5. Download the generated workflow artifact after the job finishes.

### Publish A Real Release

Option 1: enter a tag like `aida64helper-v1.0.0` in the manual workflow input.

Option 2: push a tag from git:

```powershell
git tag aida64helper-v1.0.0
git push origin aida64helper-v1.0.0
```

The workflow publishes a `win-x64` self-contained single-file zip to GitHub Releases.
