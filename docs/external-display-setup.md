# RTSS / AIDA64 / Xbox Game Bar Setup

This guide explains how to show EasyBluetooth battery status outside the main app.

## Overview

- RTSS output is written directly by EasyBluetooth and does not require AIDA64.
- AIDA64 uses the standalone EasyBluetooth AIDA64 Helper.
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

## AIDA64 Helper

1. In EasyBluetooth, enable `Settings -> Advanced Features (PRO) -> Unified Standard Data API Settings`.
2. Keep the default local API address unless you already changed it: `http://127.0.0.1:18080/api/v1/status`.
3. Download EasyBluetooth AIDA64 Helper from GitHub Releases: https://github.com/xhx123/EasyBluetooth-community/releases
4. Launch the helper and confirm the API URL and token settings.
5. In AIDA64 SensorPanel or skins, read values from `HKCU\Software\FinalWire\AIDA64\ImportValues`.

Slot rules:

- `Str1..Str10`: display text.
- `DW1..DW10`: primary numeric battery value.
- Free users write only the first visible device.
- VIP users can write up to the first 10 visible devices.
- Unknown or unsupported battery values write display text with `--` semantics and numeric value `0`.

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
- AIDA64 is blank: make sure the Unified Standard Data API is enabled, the helper can reach the local URL, and your SensorPanel reads the correct ImportValues slots.
- Token errors: copy the token from EasyBluetooth's Unified Standard Data API settings into the helper.
- Game Bar shows no devices: make sure the Unified Standard Data API is enabled, EasyBluetooth is running, and the device is visible in the main EasyBluetooth window.
