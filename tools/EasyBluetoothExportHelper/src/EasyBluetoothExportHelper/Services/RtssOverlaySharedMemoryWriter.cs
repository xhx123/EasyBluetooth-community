using EasyBluetooth.DisplayExport;
using System.IO.MemoryMappedFiles;
using System.Text;

namespace EasyBluetooth.Aida64Helper.Services;

internal sealed class RtssOverlaySharedMemoryWriter : IDisposable
{
    private const string MapName = "AIDA64_SensorValues";
    private const int MapCapacity = 256 * 1024;

    private MemoryMappedFile? _sharedMemory;
    private MemoryMappedViewAccessor? _accessor;

    public void WriteDevices(IReadOnlyList<DisplayDeviceInfo> devices)
    {
        EnsureOpened();
        string xml = DisplayExportFormatter.BuildRtssOverlayEditorAida64Xml(devices);
        WriteNullTerminatedAnsi(xml);
    }

    public void Clear()
    {
        if (_accessor != null)
        {
            WriteNullTerminatedAnsi(string.Empty);
        }

        DisposeMapping();
    }

    public void Dispose()
    {
        Clear();
    }

    private void EnsureOpened()
    {
        if (_sharedMemory != null && _accessor != null)
        {
            return;
        }

        DisposeMapping();
        _sharedMemory = MemoryMappedFile.CreateOrOpen(MapName, MapCapacity, MemoryMappedFileAccess.ReadWrite);
        _accessor = _sharedMemory.CreateViewAccessor(0, MapCapacity, MemoryMappedFileAccess.Write);
    }

    private void WriteNullTerminatedAnsi(string text)
    {
        if (_accessor == null)
        {
            return;
        }

        byte[] buffer = new byte[MapCapacity];
        byte[] payload = Encoding.Default.GetBytes(text);
        int count = Math.Min(payload.Length, MapCapacity - 1);
        Array.Copy(payload, buffer, count);
        _accessor.WriteArray(0, buffer, 0, buffer.Length);
        _accessor.Flush();
    }

    private void DisposeMapping()
    {
        _accessor?.Dispose();
        _accessor = null;

        _sharedMemory?.Dispose();
        _sharedMemory = null;
    }
}
