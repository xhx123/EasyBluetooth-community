# Device Adaptation Guide

## 1. Purpose
This guide explains how to request support for devices that are not fully recognized by EasyBluetooth, especially vendor-specific 2.4G peripherals.

## 2. Before you submit
Prepare the following:
- Device brand and exact model name
- Connection type (Bluetooth / 2.4G receiver)
- Windows version and EasyBluetooth version
- A brief description of the expected vs actual battery behavior

## 3. Submit an adaptation request
1. Go to: https://github.com/xhx123/EasyBluetooth-community/issues/new?template=device-adaptation-request.yml
2. The page opens directly with the device adaptation template.
3. Fill all required fields as completely as possible.

## 3.1 Adaptation reward policy
- If an unsupported protocol is successfully adapted and validated, eligible contributors may receive a 2.4G Protocol Unlock reward package.
- Final reward confirmation is based on maintainer review and reproducible validation results.

## 4. Capture and diagnostic data
If requested by maintainers, include:
- Packet capture output from the USB/Bluetooth diagnostic workflow
- App logs relevant to pairing and battery parsing
- Repro steps (from startup to failure)

## 5. Privacy and safety notes
- Only share logs/captures needed for protocol analysis.
- Remove personal account information before uploading files.
- Do not include unrelated sensitive data.

## 6. Validation flow
After submission:
1. Maintainers review your issue.
2. Community members may test parser candidates.
3. Support status is updated once telemetry parsing is stable.

## 7. Contribution tips
- Attach clear screenshots if UI state helps debugging.
- Test with both low and high battery states when possible.
- Follow up in the same issue to keep context centralized.
