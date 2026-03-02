# EasyBluetooth User Guide

## 1. What EasyBluetooth does
EasyBluetooth is a lightweight Windows battery monitor for Bluetooth and selected 2.4G devices. It provides one dashboard for battery level, charging status, and quick device checks.

## 2. Installation
1. Open the Microsoft Store page: https://apps.microsoft.com/detail/9PC6W3425GXP
2. Install EasyBluetooth.
3. Launch the app and keep it running in the system tray.

## 3. First-time setup
1. Connect your device in Windows Bluetooth settings (or 2.4G mode for supported 2.4G devices).
2. Open EasyBluetooth and allow initial device scan.

## 4. Daily workflow
- Check battery percentage and charging state from the main panel.
- Use tray interactions for quick visibility without opening the full window.
- Use widget view if enabled by your current plan/features.

## 5. Themes and skins
- The app supports visual customization features.
- Availability depends on your current plan (Free vs unlocked capabilities).

## 6. If battery does not appear
- Confirm the device is connected and actively reporting in Windows.
- Verify that the required 2.4G device plugin has been enabled in the plugin marketplace.
- Some devices do not expose battery data through standard BLE services.
- For unsupported vendor protocols, submit a request in the community repo.

## 7. Troubleshooting quick checklist
- Restart Bluetooth on Windows and reconnect the device.
- Reopen EasyBluetooth and wait for a fresh scan.
- Update EasyBluetooth to the latest release.
- If support is still unavailable, submit an adaptation request with today's logs and packet capture details.

## 8. More help
- FAQ: ./faq.md
- Device adaptation workflow: ./device-adaptation-guide.md
- Community adaptation template: https://github.com/xhx123/EasyBluetooth-community/issues/new?template=device-adaptation-request.yml
