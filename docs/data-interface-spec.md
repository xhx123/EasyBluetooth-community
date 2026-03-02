# Data Interface Spec (Draft)

## 1. Scope
This document defines the current high-level data fields used by EasyBluetooth for battery and connection telemetry.

## 2. Device identity fields
- `deviceId`: Stable internal identifier (string)
- `deviceName`: Display name shown in UI (string)
- `brand`: Vendor name when available (string)
- `connectionType`: `bluetooth` or `2.4g` (string)

## 3. Telemetry fields
- `batteryPercent`: 0-100 integer when available
- `batteryState`: `charging` / `discharging` / `full` / `unknown`
- `isConnected`: Boolean connection state
- `lastSeenAt`: Last telemetry update timestamp (ISO 8601 string)

## 4. Capability fields
- `supportsProtocolUnlock`: Boolean
- `supportsWidgetActions`: Boolean
- `supportsThemeFeatures`: Boolean

## 5. Parsing reliability
- `confidence`: Numeric or enum confidence level from parser pipeline
- `source`: `ble-standard` / `vendor-protocol` / `system-api`

## 6. Known limitations
- Not all devices expose battery services over standard BLE.
- Vendor protocol parsing may vary by firmware revision.
- Missing telemetry should be represented as `unknown` instead of synthetic values.

## 7. Backward compatibility
When adding new fields:
1. Keep existing keys stable.
2. Introduce optional fields first.
3. Avoid breaking parser behavior for current devices.

## 8. Status
This spec is a community draft for documentation and integration alignment. Exact runtime models may evolve with future releases.
