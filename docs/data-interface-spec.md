# Unified Standard Data API (VIP) — Usage Guide
Welcome — this guide explains how to use the Unified Standard Data API to expose device status from EasyBluetooth to third-party tools (skins, plugins, external apps). For commercial integration, please contact the author for authorization.

## 1. What this does
The Unified Standard Data API provides device status from EasyBluetooth as JSON for use by third-party software or hardware (for example Rainmeter, Wallpaper Engine, Stream Deck).

- Permission: VIP only
- Data source: in-memory application snapshot (no battery level scan on every request)
- Default binding: localhost (127.0.0.1)

## 2. Enable in Settings
Open: Settings -> Advanced Features -> Unified Standard Data API Settings (button)

Clicking this button opens a separate window where you can toggle the API and configure parameters.

Recommended configuration:
1. Enable `Enable Unified Standard Data API`
2. Keep port at the default `18080`
3. Scope: `Localhost only`
4. If you're concerned about other processes reading it, enable `Token authentication` and set a random token

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
- `name`: display name
- `source`: data source (e.g. `classic`, `ble`, `airpods`, `logitech`)
- `status`: `online` / `offline` / `sleeping` / `unknown`
- `connectionStatus`: internal connection status text
- `battery`: main battery level (0–100) or `null` if unknown
- `isCharging`: whether charging
- `isSleeping`: whether sleeping
- `isBatteryUnsupported`: whether device does not support standard battery reading
- `batteryLastUpdatedUtc`: battery last updated time (UTC ISO 8601), aligned with the app UI's battery update semantics
- `airPodsLeftBattery` / `airPodsRightBattery` / `airPodsCaseBattery`: AirPods sub-battery levels (`null` if unknown)

Visibility rule:
- `devices` only includes devices visible in the main UI. User-hidden devices are excluded from API results.

### 4.1 Full device object (example)

```json
{
  "id": "a1b2c3d4e5f6",
  "name": "My AirPods Pro",
  "source": "airpods",
  "status": "online",
  "connectionStatus": "CONNECTED",
  "battery": 78,
  "isCharging": false,
  "isSleeping": false,
  "isBatteryUnsupported": false,
  "batteryLastUpdatedUtc": "2026-02-13T09:58:25+00:00",
  "airPodsLeftBattery": 80,
  "airPodsRightBattery": 78,
  "airPodsCaseBattery": 62
}
```

### 4.2 Field definitions (recommended for third parties)

| Field | Type | Values / Range | Required | Description |
|------|------|----------------|----------|-------------|
| id | String | 12-character hex short string | Yes | Obfuscated device ID, for local matching only |
| name | String | any string | Yes | Device display name |
| source | String | classic/ble/airpods/xbox/playstation/logitech/razer/windowsbattery/rog/atk/hyperx/rapoo/mchose/unknown | Yes | Data source |
| status | String | online/offline/sleeping/unknown | Yes | Unified connection state |
| connectionStatus | String | e.g. CONNECTED | Yes | Raw connection status text for debugging |
| battery | Integer/Null | 0–100 or null | Yes | Primary battery; use `null` when unknown |
| isCharging | Boolean | true/false | Yes | Charging state |
| isSleeping | Boolean | true/false | Yes | Sleeping state |
| isBatteryUnsupported | Boolean | true/false | Yes | Whether device lacks standard battery support |
| batteryLastUpdatedUtc | String/Null | ISO 8601 UTC or null | Yes | Last battery update time; aligned with the app UI "updated at" semantics. Cache backfill does not refresh this time |
| airPodsLeftBattery | Integer/Null | 0–100 or null | Yes | Left AirPod battery; non-AirPods devices usually null |
| airPodsRightBattery | Integer/Null | 0–100 or null | Yes | Right AirPod battery; non-AirPods devices usually null |
| airPodsCaseBattery | Integer/Null | 0–100 or null | Yes | AirPods case battery; non-AirPods devices usually null |

### 4.3 Robustness recommendations
- Allow unknown future fields for forward compatibility
- Treat `battery: null` explicitly — do not treat it as `0`
- Provide default branches for unknown `source` and `status`
- `batteryLastUpdatedUtc` may be `null`; avoid forcing fallback to request time
- AirPods sub-fields may be `null`; UI should support hiding these values

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
- Adding non-breaking fields keeps the schemaVersion unchanged
- Only breaking changes (remove/rename fields, change types/semantics) will bump to `2`

## 7. appVersion
`appVersion` is the application version (for example `3.0.1`) and is useful for:
- Third-party skins that need compatibility branches
- Troubleshooting and user feedback
- Correlating logs with API outputs

Is it required?
- For simple battery-only displays, it is not mandatory
- For long-term compatibility, we recommend keeping `appVersion`
- 
## 8.Tips
- If using token: send token in header `X-Api-Token`
- For multiple devices: iterate `devices` and match by `name` or `id`
- If LAN mode is enabled, be mindful of Windows URLACL and firewall rules
- 
## 9. User-facing & privacy notes
This guide can be shared with end users. The API avoids exposing high-sensitivity data:

- It does not return full MAC addresses
- It does not return full devicePath
- `id` is a one-way obfuscated short identifier

Edge cases to consider:
- `name` may include user-provided content (e.g. real names). Mask when publishing screenshots
- If enabling LAN binding, enable Token authentication
- Treat the token as sensitive and do not share screenshots that reveal it

## 10. "Access denied" when enabling LAN (URLACL)
If you encounter errors like:
- "Startup failed: Access denied."
- `System.Net.HttpListenerException`

This typically means the Windows URLACL is not granted for the user, preventing `http://+:PORT/` binding.

### 10.1 In-app one-click fix (recommended)Open Settings -> Advanced Features (PRO) -> Unified Standard Data API Settings and click:

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

If LAN access still fails, ensure the firewall inbound rule exists:

```bat
netsh advfirewall firewall add rule name="EasyBluetooth LAN 18081" dir=in action=allow protocol=TCP localport=18081 profile=any
```
