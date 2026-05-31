# EasyBluetoothExportHelper

Open-source companion tool for exporting EasyBluetooth device data to AIDA64 SensorPanel and RTSS Overlay Editor.

## What It Does

- Polls the local EasyBluetooth Unified Standard Data API
- Writes up to 10 device slots into `HKCU\Software\FinalWire\AIDA64\ImportValues`
- Writes RTSS Overlay Editor sources through the AIDA64 shared memory map `AIDA64_SensorValues`
- Exports device name, numeric battery, charging state, and sleeping state

## Requirements

- Windows
- EasyBluetooth with Unified Standard Data API enabled
- VIP active if your EasyBluetooth setup requires the API entitlement
- AIDA64 installed if you use SensorPanel output
- RTSS installed if you use Overlay Editor data sources

Do not enable RTSS Overlay Editor output while another tool is writing real AIDA64 shared memory data, because both use the same `AIDA64_SensorValues` map name.

## RTSS Overlay Editor Sources

For each visible device, up to 10 devices:

- `DeviceN Name`
- `DeviceN Battery`
- `DeviceN Status`
- `DeviceN Charging`
- `DeviceN Sleeping`

`DeviceN Battery`, `DeviceN Charging`, and `DeviceN Sleeping` are numeric. `DeviceN Name` and `DeviceN Status` are strings; use `%s` as the RTSS source format when rendering them as text.

## Project Layout

- Source project: `src/EasyBluetoothExportHelper/EasyBluetoothExportHelper.csproj`
- GitHub release workflow: `.github/workflows/release-exporthelper.yml`

## Local Build

```powershell
dotnet build .\tools\EasyBluetoothExportHelper\src\EasyBluetoothExportHelper\EasyBluetoothExportHelper.csproj -c Debug
```

## Local Publish

```powershell
dotnet publish .\tools\EasyBluetoothExportHelper\src\EasyBluetoothExportHelper\EasyBluetoothExportHelper.csproj `
  -c Release `
  -f net8.0-windows `
  -r win-x64 `
  --self-contained true `
  -p:PublishSingleFile=true `
  -p:IncludeNativeLibrariesForSelfExtract=true `
  -p:DebugSymbols=false `
  -p:DebugType=None `
  -p:PublishReadyToRun=false
```

Public release packages intentionally exclude `.pdb` symbol files, because end users do not need them to run the helper.

## GitHub Actions Usage

1. Open the repository `Actions` tab.
2. Select `Release EasyBluetoothExportHelper`.
3. Click `Run workflow`.
4. Leave `release_tag` empty if you only want a build artifact.
5. To publish a release, enter a tag like `exporthelper-v1.0.0`, or push that tag:

```powershell
git tag exporthelper-v1.0.0
git push origin exporthelper-v1.0.0
```

The workflow publishes a `win-x64` self-contained single-file zip to GitHub Releases.
