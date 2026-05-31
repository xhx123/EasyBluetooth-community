using EasyBluetooth.DisplayExport;
using Microsoft.Win32;

namespace EasyBluetooth.Aida64Helper.Services;

internal sealed class Aida64RegistryWriter
{
    private const string RegistryPath = @"Software\FinalWire\AIDA64\ImportValues";

    public void WriteSlots(IReadOnlyList<Aida64ImportSlot> slots)
    {
        using RegistryKey key = Registry.CurrentUser.CreateSubKey(RegistryPath, writable: true)
            ?? throw new InvalidOperationException("Unable to open AIDA64 import registry path.");

        for (int index = 1; index <= DisplayExportFormatter.MaxAida64Slots; index++)
        {
            Aida64ImportSlot? slot = slots.FirstOrDefault(item => item.Index == index);
            key.SetValue($"Str{index}", slot?.StringValue ?? string.Empty, RegistryValueKind.String);
            key.SetValue($"DW{index}", slot?.NumericValue ?? 0, RegistryValueKind.DWord);
        }
    }

    public static bool IsAida64Running()
    {
        foreach (string processName in new[] { "aida64", "aida64extreme", "aida64engineering", "aida64business" })
        {
            if (System.Diagnostics.Process.GetProcessesByName(processName).Length > 0)
            {
                return true;
            }
        }

        return false;
    }
}
