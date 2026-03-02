# EasyBluetooth FAQ

## Why is my battery level missing?
Some devices do not expose battery data through standard Bluetooth services. In these cases, EasyBluetooth may require vendor-specific protocol support.

## Does EasyBluetooth support 2.4G receivers?
Yes, selected brands are supported through protocol adaptation. Coverage expands over time via community requests and validation.

## Do I need to keep OEM software installed?
Not always. Many users can replace heavy OEM suites for battery visibility. Some advanced vendor-only controls may still require original tools.

## Is there a free version?
Yes. You can start with free features, then optionally unlock VIP and 2.4G protocol capabilities via one-time purchase options.

## How do I request support for a new device?
Open an issue in the community repository using the adaptation request template and include model details plus logs/capture data when possible.

## Is my data uploaded to cloud by default?
EasyBluetooth is designed around local desktop monitoring workflows. Refer to the full privacy policy for exact details and scope.

## What should I include in a bug report?
- Device model and brand
- Connection mode (Bluetooth or 2.4G)
- Windows version
- EasyBluetooth version
- Steps to reproduce
- Screenshots/logs if available

## Where can I find official docs?
- User guide: ./user-guide.md
- Adaptation guide: ./device-adaptation-guide.md
- Data interface draft: ./data-interface-spec.md
