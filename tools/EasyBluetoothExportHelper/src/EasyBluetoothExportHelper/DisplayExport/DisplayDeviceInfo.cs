namespace EasyBluetooth.DisplayExport;

public sealed class DisplayDeviceInfo
{
    public string Id { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public string? RenamedName { get; init; }

    public string Status { get; init; } = "unknown";

    public string ConnectionStatus { get; init; } = string.Empty;

    public int? Battery { get; init; }

    public bool IsCharging { get; init; }

    public bool IsSleeping { get; init; }

    public bool IsBatteryUnsupported { get; init; }

    public int? AirPodsLeftBattery { get; init; }

    public int? AirPodsRightBattery { get; init; }

    public int? AirPodsCaseBattery { get; init; }

    public string DisplayName => string.IsNullOrWhiteSpace(RenamedName) ? Name : RenamedName!;

    public int Aida64NumericBattery => Battery ?? 0;
}
