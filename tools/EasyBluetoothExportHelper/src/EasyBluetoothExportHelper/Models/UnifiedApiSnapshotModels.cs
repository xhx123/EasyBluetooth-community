namespace EasyBluetooth.Aida64Helper.Models;

public sealed class UnifiedApiStatusPayload
{
    public int SchemaVersion { get; set; }

    public DateTimeOffset GeneratedAtUtc { get; set; }

    public string AppVersion { get; set; } = string.Empty;

    public List<UnifiedApiDeviceSnapshot> Devices { get; set; } = new();
}

public sealed class UnifiedApiDeviceSnapshot
{
    public string Id { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string? RenamedName { get; set; }

    public string DeviceType { get; set; } = "general";

    public string Source { get; set; } = "unknown";

    public string Status { get; set; } = "unknown";

    public string ConnectionStatus { get; set; } = string.Empty;

    public int? Battery { get; set; }

    public bool IsCharging { get; set; }

    public bool IsSleeping { get; set; }

    public bool IsBatteryUnsupported { get; set; }

    public bool IsShownInTray { get; set; }

    public int? AirPodsLeftBattery { get; set; }

    public int? AirPodsRightBattery { get; set; }

    public int? AirPodsCaseBattery { get; set; }

    public DateTimeOffset? BatteryLastUpdatedUtc { get; set; }
}
