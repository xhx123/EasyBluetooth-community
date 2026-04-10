using EasyBluetooth.Aida64Helper.Models;
using EasyBluetooth.DisplayExport;
using System.Text.Json;

namespace EasyBluetooth.Aida64Helper.Services;

internal sealed class UnifiedApiClient
{
    private static readonly HttpClient HttpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(3)
    };

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public async Task<UnifiedApiFetchResult> FetchAsync(string apiUrl, string apiToken, CancellationToken cancellationToken)
    {
        if (!Uri.TryCreate(apiUrl, UriKind.Absolute, out var uri))
        {
            return UnifiedApiFetchResult.InvalidUrl();
        }

        try
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, uri);
            if (!string.IsNullOrWhiteSpace(apiToken))
            {
                request.Headers.TryAddWithoutValidation("X-Api-Token", apiToken.Trim());
            }

            using var response = await HttpClient.SendAsync(request, cancellationToken).ConfigureAwait(false);
            string payload = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

            UnifiedApiEnvelope<UnifiedApiStatusPayload>? envelope = null;
            if (!string.IsNullOrWhiteSpace(payload))
            {
                envelope = JsonSerializer.Deserialize<UnifiedApiEnvelope<UnifiedApiStatusPayload>>(payload, JsonOptions);
            }

            if (!response.IsSuccessStatusCode)
            {
                return response.StatusCode switch
                {
                    System.Net.HttpStatusCode.Unauthorized => UnifiedApiFetchResult.Unauthorized(),
                    System.Net.HttpStatusCode.Forbidden => UnifiedApiFetchResult.Forbidden(),
                    _ => UnifiedApiFetchResult.ServerError(envelope?.Message ?? response.ReasonPhrase ?? "HTTP error")
                };
            }

            if (envelope?.Code != 200 || envelope.Data?.Devices == null)
            {
                return UnifiedApiFetchResult.InvalidResponse(envelope?.Message ?? "Missing data");
            }

            var devices = envelope.Data.Devices
                .Where(device => !string.IsNullOrWhiteSpace(device.Name))
                .Select(device => new DisplayDeviceInfo
                {
                    Id = device.Id,
                    Name = device.Name,
                    RenamedName = device.RenamedName,
                    Status = device.Status,
                    ConnectionStatus = device.ConnectionStatus,
                    Battery = device.Battery,
                    IsCharging = device.IsCharging,
                    IsSleeping = device.IsSleeping,
                    IsBatteryUnsupported = device.IsBatteryUnsupported,
                    AirPodsLeftBattery = device.AirPodsLeftBattery,
                    AirPodsRightBattery = device.AirPodsRightBattery,
                    AirPodsCaseBattery = device.AirPodsCaseBattery
                })
                .ToList();

            return UnifiedApiFetchResult.Success(devices);
        }
        catch (HttpRequestException ex)
        {
            return UnifiedApiFetchResult.ConnectionFailed(ex.Message);
        }
        catch (TaskCanceledException ex)
        {
            return UnifiedApiFetchResult.ConnectionFailed(ex.Message);
        }
        catch (JsonException ex)
        {
            return UnifiedApiFetchResult.InvalidResponse(ex.Message);
        }
    }
}

internal enum UnifiedApiFetchStatus
{
    Success,
    InvalidUrl,
    ConnectionFailed,
    Unauthorized,
    Forbidden,
    InvalidResponse,
    ServerError
}

internal sealed class UnifiedApiFetchResult
{
    public required UnifiedApiFetchStatus Status { get; init; }

    public required string Detail { get; init; }

    public required IReadOnlyList<DisplayDeviceInfo> Devices { get; init; }

    public static UnifiedApiFetchResult Success(IReadOnlyList<DisplayDeviceInfo> devices)
        => new()
        {
            Status = UnifiedApiFetchStatus.Success,
            Detail = string.Empty,
            Devices = devices
        };

    public static UnifiedApiFetchResult InvalidUrl()
        => Create(UnifiedApiFetchStatus.InvalidUrl, string.Empty);

    public static UnifiedApiFetchResult ConnectionFailed(string detail)
        => Create(UnifiedApiFetchStatus.ConnectionFailed, detail);

    public static UnifiedApiFetchResult Unauthorized()
        => Create(UnifiedApiFetchStatus.Unauthorized, string.Empty);

    public static UnifiedApiFetchResult Forbidden()
        => Create(UnifiedApiFetchStatus.Forbidden, string.Empty);

    public static UnifiedApiFetchResult InvalidResponse(string detail)
        => Create(UnifiedApiFetchStatus.InvalidResponse, detail);

    public static UnifiedApiFetchResult ServerError(string detail)
        => Create(UnifiedApiFetchStatus.ServerError, detail);

    private static UnifiedApiFetchResult Create(UnifiedApiFetchStatus status, string detail)
        => new()
        {
            Status = status,
            Detail = detail,
            Devices = Array.Empty<DisplayDeviceInfo>()
        };
}
