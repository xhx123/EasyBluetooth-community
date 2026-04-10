namespace EasyBluetooth.Aida64Helper.Models;

public sealed class UnifiedApiEnvelope<T>
{
    public int Code { get; set; }

    public string Message { get; set; } = string.Empty;

    public T? Data { get; set; }
}
