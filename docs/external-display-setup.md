# RTSS / AIDA64 / Xbox Game Bar Setup

This guide explains how to show EasyBluetooth battery status outside the main app.

## Overview

- RTSS in-game OSD output is written directly by EasyBluetooth and does not require AIDA64.
- RTSS Overlay Editor data sources and AIDA64 SensorPanel use the standalone EasyBluetoothExportHelper.
- Xbox Game Bar uses the EasyBluetooth Game Bar widget while the main EasyBluetooth app provides the local read-only data source through the Unified Standard Data API switch.
- Free users can display the first visible device. VIP users can display all visible devices.
- Restricted 2.4G receiver groups still follow the 2.4G Protocol Unlock rules.

## RTSS In-Game OSD

1. Install RTSS and keep it running in the background.
2. Open EasyBluetooth.
3. Go to `Settings -> Game OSD / AIDA64 Export`.
4. Turn on `Enable RTSS OSD`.
5. Start a game. EasyBluetooth writes battery text into the RTSS overlay.

Notes:

- Unknown battery values are shown as `--`.
- Charging devices append `CHG`.
- Sleeping devices append `SLP`.
- AirPods details can include `L / R / C` values when available.

## EasyBluetoothExportHelper

1. In EasyBluetooth, enable `Settings -> Advanced Features (PRO) -> Unified Standard Data API Settings`.
2. Keep the default local API address unless you already changed it: `http://127.0.0.1:18080/api/v1/status`.
3. Download EasyBluetoothExportHelper from GitHub Releases: https://github.com/xhx123/EasyBluetooth-community/releases/latest
4. In `Settings -> Game OSD / AIDA64 Export`, click `Select Helper...` and choose `EasyBluetoothExportHelper.exe`.
5. Launch the helper and confirm the API URL, token, and output mode settings.
6. For AIDA64 SensorPanel or skins, read values from `HKCU\Software\FinalWire\AIDA64\ImportValues`.
7. For RTSS Overlay Editor, add data sources from the `AIDA64` provider.

Slot rules:

- `Str1..Str10`: display text.
- `DW1..DW10`: primary numeric battery value.
- Free users write only the first visible device.
- VIP users can write up to the first 10 visible devices.
- Unknown or unsupported battery values write display text with `--` semantics and numeric value `0`.

RTSS Overlay Editor sources:

- `DeviceN Name`: string device display name.
- `DeviceN Battery`: numeric battery value, suitable for bars.
- `DeviceN Status`: `Charging`, `Sleeping`, `Charging/Sleeping`, or visually blank.
- `DeviceN Charging`: numeric `0/1`.
- `DeviceN Sleeping`: numeric `0/1`.

Do not enable RTSS Overlay Editor output together with another tool writing real AIDA64 shared memory data, because both use the `AIDA64_SensorValues` map name.

## Xbox Game Bar Widget

1. Install and run EasyBluetooth.
2. Enable `Settings -> Advanced Features (PRO) -> Unified Standard Data API Settings`.
3. Press `Win + G` to open Xbox Game Bar.
4. Open the Widget menu or widget store.
5. Search for `EasyBluetooth Game Bar`.
6. Add or pin the `EasyBluetooth Battery` widget.

The widget needs the Unified Standard Data API enabled and the main EasyBluetooth app to keep running. If the widget is waiting for data, return to EasyBluetooth and refresh devices or battery levels first.

<!-- TODO: Replace the search guidance above with the standalone EasyBluetooth Game Bar Store URL when the public link is available. -->

## Troubleshooting

- RTSS is blank: make sure RTSS is installed, running, and RTSS OSD is enabled in EasyBluetooth.
- Export Helper is blank: make sure the Unified Standard Data API is enabled, the helper can reach the local URL, and the needed output mode is enabled.
- AIDA64 is blank: make sure your SensorPanel reads the correct ImportValues slots.
- RTSS Overlay Editor has no sources: keep EasyBluetoothExportHelper running, enable `RTSS Overlay Editor`, then add sources from the `AIDA64` provider.
- Token errors: copy the token from EasyBluetooth's Unified Standard Data API settings into the helper.
- Game Bar shows no devices: make sure the Unified Standard Data API is enabled, EasyBluetooth is running, and the device is visible in the main EasyBluetooth window.
