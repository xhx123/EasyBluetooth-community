# Unified Standard Data API (VIP) - Usage Guide
Welcome - this guide explains how to use the Unified Standard Data API to expose device status from EasyBluetooth to third-party tools (skins, plugins, external apps). For commercial integration, please contact the author for authorization.

## 1. What this does
The Unified Standard Data API provides device status from EasyBluetooth as JSON for use by third-party software or hardware (for example Rainmeter, Wallpaper Engine, Stream Deck).

- Permission: VIP only
- Data source: in-memory application snapshot (no battery scan on every request)
- Default binding: localhost (`127.0.0.1`)

## 2. Enable in Settings
Open: `Settings -> Advanced Features (PRO) -> Unified Standard Data API Settings`

Clicking this button opens a separate window where you can enable the API and configure parameters.

Recommended configuration:
1. Enable `Enable Unified Standard Data API`
2. Keep port at the default `18080`
3. Scope: `Localhost only`
4. If you are concerned about other local processes reading it, enable `Token authentication` and set a random token

## 3. Endpoints
- Status endpoint: `GET /api/v1/status`
- Example full local URL: `http://127.0.0.1:18080/api/v1/status`

## 4. Response format
All responses use the unified format:

```json
{
  "code": 200,
  "message": "OK",
  "data": {
    "schemaVersion": 1,
    "generatedAtUtc": "2026-02-13T10:00:00+00:00",
    "appVersion": "3.0.1",
    "devices": []
  }
}
```

### `devices` field
Each device item includes:
- `id`: obfuscated device identifier (short hash, original MAC/devicePath not exposed)
- `name`: original device name from the system/protocol, not affected by user rename
- `renamedName`: user-renamed display name, or `null` if the user has not renamed it
- `deviceType`: device category, using the user's selected preset icon category when available, otherwise auto-detected from the original device name; fixed values: `general` / `headphones` / `mouse` / `keyboard` / `speaker` / `gamepad`
- `source`: data source (for example `classic`, `ble`, `airpods`, `logitech`)
- `status`: `online` / `offline` / `unknown`
- `connectionStatus`: internal connection status text
- `battery`: primary battery level (`0-100`), or `null` when unknown and no previously valid battery value can be retained
- `isCharging`: whether charging
- `isSleeping`: whether sleeping; `true` does not imply `battery` must be `null`
- `isBatteryUnsupported`: whether the device does not support standard battery reading
- `isShownInTray`: whether the user explicitly marked this device as "show in tray"
- `batteryLastUpdatedUtc`: battery last updated time (UTC ISO 8601), aligned with the app UI's "updated at" semantics; sleep-state refreshes do not change this timestamp, but may retain the last known value
- `airPodsLeftBattery` / `airPodsRightBattery` / `airPodsCaseBattery`: AirPods sub-battery levels (`null` if unknown)

Visibility rule:
- `devices` only includes devices visible in the main UI. User-hidden devices are excluded from API results.

### 4.1 Full device object (example)

```json
{
  "id": "a1b2c3d4e5f6",
  "name": "My AirPods Pro",
  "renamedName": "Commute Earbuds",
  "deviceType": "headphones",
  "source": "airpods",
  "status": "online",
  "connectionStatus": "CONNECTED",
  "battery": 78,
  "isCharging": false,
  "isSleeping": false,
  "isBatteryUnsupported": false,
  "isShownInTray": true,
  "batteryLastUpdatedUtc": "2026-02-13T09:58:25+00:00",
  "airPodsLeftBattery": 80,
  "airPodsRightBattery": 78,
  "airPodsCaseBattery": 62
}
```

### 4.1.1 Sleeping device example (retaining the last valid battery)

```json
{
  "id": "b7c8d9e0f1a2",
  "name": "Razer Viper V3 Pro",
  "renamedName": null,
  "deviceType": "mouse",
  "source": "razer",
  "status": "online",
  "connectionStatus": "CONNECTED",
  "battery": 24,
  "isCharging": false,
  "isSleeping": true,
  "isBatteryUnsupported": false,
  "isShownInTray": false,
  "batteryLastUpdatedUtc": "2026-03-11T02:15:43+00:00",
  "airPodsLeftBattery": null,
  "airPodsRightBattery": null,
  "airPodsCaseBattery": null
}
```

### 4.2 Field definitions (recommended for third parties)

| Field | Type | Values / Range | Required | Description |
|------|------|----------------|----------|-------------|
| id | String | 12-character hex short string | Yes | Obfuscated device ID, for local matching only |
| name | String | any string | Yes | Original device name, not affected by user rename |
| renamedName | String/Null | any string or null | Yes | User-renamed display name; `null` if not renamed |
| deviceType | String | general/headphones/mouse/keyboard/speaker/gamepad | Yes | Device category; uses the user's selected preset icon category when available, otherwise auto-detected from the original device name |
| source | String | classic/ble/airpods/xbox/playstation/logitech/razer/windowsbattery/rog/atk/hyperx/rapoo/mchose/valkyrie/unknown | Yes | Data source |
| status | String | online/offline/unknown | Yes | Unified connection-state semantics |
| connectionStatus | String | for example `CONNECTED` | Yes | Raw connection status text for debugging |
| battery | Integer/Null | `0-100` or null | Yes | Primary battery. `null` means there is no currently valid battery value and no previously valid retained value |
| isCharging | Boolean | true/false | Yes | Charging state |
| isSleeping | Boolean | true/false | Yes | Sleeping state; `true` does not imply `battery` must be `null` |
| isBatteryUnsupported | Boolean | true/false | Yes | Whether the device lacks standard battery support |
| isShownInTray | Boolean | true/false | Yes | Whether the device is currently explicitly marked by the user as "show in tray"; `false` when not explicitly set |
| batteryLastUpdatedUtc | String/Null | ISO 8601 UTC or null | Yes | Last battery update time; aligned with the app UI's "updated at" semantics. Sleep-state refreshes do not update this timestamp, but may retain the last known value |
| airPodsLeftBattery | Integer/Null | `0-100` or null | Yes | Left AirPod battery; usually `null` for non-AirPods devices |
| airPodsRightBattery | Integer/Null | `0-100` or null | Yes | Right AirPod battery; usually `null` for non-AirPods devices |
| airPodsCaseBattery | Integer/Null | `0-100` or null | Yes | AirPods case battery; usually `null` for non-AirPods devices |

### 4.3 Robustness recommendations
- Allow unknown future fields for forward compatibility
- Treat `battery: null` explicitly; do not treat it as `0`
- Provide default branches for unknown `source` and `status`
- Provide a default branch for unknown `deviceType`; it is intended for icon/category mapping
- `batteryLastUpdatedUtc` may be `null`; avoid forcing fallback to request time
- AirPods sub-fields may be `null`; UI should support hiding these values
- If you want the name users actually see in the app, prefer `renamedName ?? name`

## 5. Error codes
- `401`: Authentication failed (token enabled but missing or incorrect)
- `403`: Non-VIP user attempted to call the API
- `404`: Path not found
- `405`: Method not allowed (only GET supported)
- `500`: Internal server error
- `503`: Authentication enabled but token not configured

## 6. schemaVersion
`schemaVersion` is the API schema version, not the app version.

- Current: `1`
- Adding non-breaking fields keeps `schemaVersion` unchanged
- Adding `deviceType` in this revision is non-breaking and does not bump `schemaVersion`
- Only breaking changes (remove/rename fields, change types/semantics) will bump it to `2`

## 7. appVersion
`appVersion` is the application version (for example `3.0.1`) and is useful for:

- Third-party skins that need compatibility branches
- Troubleshooting and user feedback
- Correlating logs with API outputs

Is it required?

- For simple battery-only displays, it is not mandatory
- For long-term compatibility, we recommend keeping `appVersion`

Current recommendation: keep `appVersion`, because it is not sensitive and it makes troubleshooting much easier.

## 8. Tips
- If using token: send the token in header `X-Api-Token`
- For multiple devices: prefer matching by `id`; `name` / `renamedName` are better display fields
- If LAN mode is enabled, be mindful of Windows URLACL and firewall rules

## 9. User-facing & privacy notes
This guide can be shared with end users. The API avoids exposing high-sensitivity data:

- It does not return full MAC addresses
- It does not return full `devicePath`
- `id` is a one-way obfuscated short identifier

Edge cases to consider:

- `name` is the original device name; `renamedName` may include user-provided content (for example real names or nicknames). Mask it when publishing screenshots
- If enabling LAN binding, enable Token authentication
- Treat the token as sensitive and do not share screenshots that reveal it

## 10. "Access denied" when enabling LAN (URLACL)
If you encounter errors like:

- "Startup failed: Access denied."
- `System.Net.HttpListenerException`

This typically means the Windows URLACL is not granted for the user, preventing `http://+:PORT/` binding.

### 10.1 In-app one-click fix (recommended)
Open `Settings -> Advanced Features (PRO) -> Unified Standard Data API Settings` and click:

- `One-click fix network access`

The app will request elevation and automatically execute the fix. After completion, re-enable or toggle LAN binding.

### 10.2 Manual fix (Administrator PowerShell / CMD)
Replace `18080` with the port you actually use (for example `18081`):

```bat
netsh http delete urlacl url=http://+:18080/
netsh http delete urlacl url=http://127.0.0.1:18080/
netsh http delete urlacl url=http://localhost:18080/

netsh http add urlacl url=http://+:18080/ user=Everyone
netsh http add urlacl url=http://127.0.0.1:18080/ user=Everyone
netsh http add urlacl url=http://localhost:18080/ user=Everyone

netsh advfirewall firewall add rule name="EasyBluetooth LAN 18080" dir=in action=allow protocol=TCP localport=18080 profile=any
```

Example when current user is `MYPC\Alice` and port is `18081`:

```bat
netsh http delete urlacl url=http://+:18081/
netsh http delete urlacl url=http://127.0.0.1:18081/
netsh http delete urlacl url=http://localhost:18081/

netsh http add urlacl url=http://+:18081/ user=Everyone
netsh http add urlacl url=http://127.0.0.1:18081/ user=Everyone
netsh http add urlacl url=http://localhost:18081/ user=Everyone

netsh advfirewall firewall add rule name="EasyBluetooth LAN 18081" dir=in action=allow protocol=TCP localport=18081 profile=any
```

## 11. Semantics update (2026-03)
- `battery = null` means there is currently no valid battery value, and there was also no previously valid battery value available to retain. This may be caused by a temporary read failure, a device first seen while already sleeping/idle, or a device that does not support the current read path.
- When `isSleeping = true`, if the device had a recent valid battery value, both `battery` and `batteryLastUpdatedUtc` continue returning that retained value. They stay `null` only if no valid value existed before.
- When `isBatteryUnsupported = false` and `battery = null`, the device may still be online; it only means there is no confirmed valid battery value right now. For display-only clients, rendering this as `--` is recommended.
- When `isBatteryUnsupported = true` and `battery = null`, the device exists but the current path does not support battery reading. For display-only clients, rendering this as `--` is also recommended.
- `batteryLastUpdatedUtc = null` does not mean the device is offline. It only means there is no confirmed valid battery timestamp to retain.
- Since March 2026, `isSleeping` remains a technical status flag. A sleeping device can still return its last valid battery value and timestamp, while `status` continues to represent online/offline semantics.
- `name` always represents the original device name; `renamedName` is the user-renamed display name and is `null` when not renamed.
- `deviceType` describes the device category, not an icon-source type. If the user selected a preset icon category, that category is returned first; otherwise it is auto-detected from the original device name.
- `isShownInTray = true` only means the user explicitly marked the device as "show in tray". If the tray is still using the default lowest-battery logic, all devices may be `false`.

If LAN access still fails, ensure the firewall inbound rule exists:

```bat
netsh advfirewall firewall add rule name="EasyBluetooth LAN 18081" dir=in action=allow protocol=TCP localport=18081 profile=any
```
