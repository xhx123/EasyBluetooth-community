using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasyBluetooth.DisplayExport;

public static class DisplayExportFormatter
{
    public const int MaxAida64Slots = 10;

    public static IReadOnlyList<string> BuildRtssLines(IEnumerable<DisplayDeviceInfo>? devices)
    {
        if (devices == null)
        {
            return Array.Empty<string>();
        }

        return devices
            .Where(device => device != null && !string.IsNullOrWhiteSpace(device.Name))
            .Select(FormatDeviceLine)
            .Where(line => !string.IsNullOrWhiteSpace(line))
            .ToList();
    }

    public static string BuildRtssText(IEnumerable<DisplayDeviceInfo>? devices)
    {
        var lines = BuildRtssLines(devices);
        return lines.Count == 0 ? string.Empty : string.Join("\n", lines);
    }

    public static IReadOnlyList<Aida64ImportSlot> BuildAida64Slots(IEnumerable<DisplayDeviceInfo>? devices, int maxSlots = MaxAida64Slots)
    {
        if (devices == null)
        {
            return Array.Empty<Aida64ImportSlot>();
        }

        int normalizedMaxSlots = Math.Clamp(maxSlots, 1, MaxAida64Slots);

        return devices
            .Where(device => device != null && !string.IsNullOrWhiteSpace(device.Name))
            .Take(normalizedMaxSlots)
            .Select((device, index) => new Aida64ImportSlot
            {
                Index = index + 1,
                StringValue = FormatDeviceLine(device),
                NumericValue = device.Aida64NumericBattery
            })
            .ToList();
    }

    public static string BuildRtssOverlayEditorAida64Xml(IEnumerable<DisplayDeviceInfo>? devices, int maxSlots = MaxAida64Slots)
    {
        if (devices == null)
        {
            return string.Empty;
        }

        int normalizedMaxSlots = Math.Clamp(maxSlots, 1, MaxAida64Slots);
        var builder = new StringBuilder();

        int index = 1;
        foreach (var device in devices.Where(device => device != null && !string.IsNullOrWhiteSpace(device.Name)).Take(normalizedMaxSlots))
        {
            string prefix = $"EB_D{index}";
            AppendRtssOverlaySensor(builder, $"{prefix}_NAME", $"Device{index} Name", string.IsNullOrWhiteSpace(device.DisplayName) ? device.Name : device.DisplayName);
            AppendRtssOverlaySensor(builder, $"{prefix}_BATTERY", $"Device{index} Battery", FormatRtssOverlayBattery(device));
            AppendRtssOverlaySensor(builder, $"{prefix}_STATUS", $"Device{index} Status", FormatRtssOverlayStatus(device));
            AppendRtssOverlaySensor(builder, $"{prefix}_CHARGING", $"Device{index} Charging", device.IsCharging ? "1" : "0");
            AppendRtssOverlaySensor(builder, $"{prefix}_SLEEPING", $"Device{index} Sleeping", device.IsSleeping ? "1" : "0");
            index++;
        }

        return builder.ToString();
    }

    public static string FormatDeviceLine(DisplayDeviceInfo device)
    {
        ArgumentNullException.ThrowIfNull(device);

        var builder = new StringBuilder();
        builder.Append(string.IsNullOrWhiteSpace(device.DisplayName) ? device.Name : device.DisplayName);
        builder.Append(": ");
        builder.Append(FormatBatteryValue(device.Battery, device.IsBatteryUnsupported));

        string componentSuffix = BuildAirPodsSuffix(device);
        if (!string.IsNullOrEmpty(componentSuffix))
        {
            builder.Append(' ');
            builder.Append(componentSuffix);
        }

        string stateSuffix = BuildStateSuffix(device);
        if (!string.IsNullOrEmpty(stateSuffix))
        {
            builder.Append(' ');
            builder.Append(stateSuffix);
        }

        return builder.ToString();
    }

    public static string FormatBatteryValue(int? battery, bool isBatteryUnsupported)
    {
        if (!battery.HasValue || isBatteryUnsupported)
        {
            return "--";
        }

        return $"{Math.Clamp(battery.Value, 0, 100)}%";
    }

    private static string BuildAirPodsSuffix(DisplayDeviceInfo device)
    {
        var parts = new List<string>(3);
        AppendComponent(parts, "L", device.AirPodsLeftBattery);
        AppendComponent(parts, "R", device.AirPodsRightBattery);
        AppendComponent(parts, "C", device.AirPodsCaseBattery);

        return parts.Count == 0 ? string.Empty : $"[{string.Join(" ", parts)}]";
    }

    private static void AppendComponent(ICollection<string> parts, string label, int? value)
    {
        if (!value.HasValue)
        {
            return;
        }

        parts.Add($"{label}{Math.Clamp(value.Value, 0, 100)}");
    }

    private static string BuildStateSuffix(DisplayDeviceInfo device)
    {
        var states = new List<string>(2);
        if (device.IsCharging)
        {
            states.Add("CHG");
        }

        if (device.IsSleeping)
        {
            states.Add("SLP");
        }

        return states.Count == 0 ? string.Empty : $"[{string.Join("/", states)}]";
    }

    private static void AppendRtssOverlaySensor(StringBuilder builder, string id, string label, string value)
    {
        builder
            .Append("<sensor><id>")
            .Append(EscapeRtssOverlayValue(id))
            .Append("</id><label>")
            .Append(EscapeRtssOverlayValue(label))
            .Append("</label><value>")
            .Append(EscapeRtssOverlayValue(value))
            .Append("</value></sensor>");
    }

    private static string FormatRtssOverlayBattery(DisplayDeviceInfo device)
    {
        if (!device.Battery.HasValue || device.IsBatteryUnsupported)
        {
            return "0";
        }

        return Math.Clamp(device.Battery.Value, 0, 100).ToString();
    }

    private static string FormatRtssOverlayStatus(DisplayDeviceInfo device)
    {
        if (device.IsCharging && device.IsSleeping)
        {
            return "Charging/Sleeping";
        }

        if (device.IsCharging)
        {
            return "Charging";
        }

        if (device.IsSleeping)
        {
            return "Sleeping";
        }

        return " ";
    }

    private static string EscapeRtssOverlayValue(string value)
    {
        return value
            .Replace("<", "&lt;", StringComparison.Ordinal)
            .Replace(">", "&gt;", StringComparison.Ordinal);
    }
}
