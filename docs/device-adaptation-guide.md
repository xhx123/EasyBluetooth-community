# Device Adaptation Guide

## 1. Purpose
This guide explains how to request support for vendor-specific 2.4G peripherals that are not fully recognized by EasyBluetooth. Direct Bluetooth captures can still help diagnostics, but they are not part of the 2.4G reward flow.

## 2. Before you submit
Prepare the following:
- Confirm that the official driver of the device can identify the battery, and only the device that supports the identification of the power level supports adaptation
- Device brand and exact model name
- Windows version and EasyBluetooth version
- A brief description of the expected vs actual battery behavior

## 3. Submit an adaptation request
1. Go to: https://github.com/xhx123/EasyBluetooth-community/issues/new?template=device-adaptation-request.yml
2. The page opens directly with the device adaptation template.
3. Fill all required fields as completely as possible.

## 4. Capture and diagnostic data
After a vendor-specific 2.4G adaptation request is approved, please submit:
- Battery data collected via EasyBluetooth’s USB capture helper (follow the in-app instructions for the capture process).
- At least three datasets: two discharging datasets at different battery levels and one charging dataset.
- For charging files, please charge the device first, then perform A/B snapshots again.
- Rename each file using this format: BrandModel+VID+PID+BatteryLevel+Charging/Discharging.
- The email address linked to your Microsoft account, which will be used to add you to the testing group for Microsoft Store preview builds.

## 5. Privacy and safety notes
- Only share logs/captures needed for protocol analysis.
- Remove personal account information before uploading files.
- Do not include unrelated sensitive data.

## 6. Validation flow
After submission:
1. The developer integrates the new protocol into a testing build and releases it via Microsoft Store Flighting.
2. The user is added to the test group using the Microsoft account email they provided.
3. After installing the test build, the user performs verification. If it still fails, the user submits logs and screenshots to the developer’s support email.
4. This process is repeated iteratively until the device battery level is successfully recognized.

## 7. 2.4G adaptation reward policy
- Only vendor-specific 2.4G protocol adaptations that are successfully integrated and validated may qualify for a 2.4G Protocol Unlock reward package.
- Direct Bluetooth capture submissions are diagnostics-only and do not qualify for the 2.4G Protocol Unlock reward package.
- If the device only requires adding a new VID/PID under an already supported protocol, the contribution can still be accepted and listed on the contribution wall, but it is not eligible for the 2.4G Protocol Unlock reward package.
- Final reward confirmation is based on maintainer review and reproducible validation results.
- Only the first valid data submission for each 2.4G protocol is eligible for a reward. The submission time is determined by the timestamp when the developer’s email receives the message.
