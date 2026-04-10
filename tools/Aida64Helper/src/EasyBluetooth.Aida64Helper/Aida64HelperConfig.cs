using EasyBluetooth.Aida64Helper.Services;

namespace EasyBluetooth.Aida64Helper;

public sealed class Aida64HelperConfig
{
    public const string DefaultApiUrlValue = "http://127.0.0.1:18080/api/v1/status";
    public const int DefaultPollIntervalSecondsValue = 5;
    public const string DefaultLanguagePreferenceValue = "en-US";

    public string ApiUrl { get; set; } = DefaultApiUrlValue;

    public string ApiToken { get; set; } = string.Empty;

    public int PollIntervalSeconds { get; set; } = DefaultPollIntervalSecondsValue;

    public string LanguagePreference { get; set; } = DefaultLanguagePreferenceValue;

    public bool IsOutputEnabled { get; set; } = true;

    public string LastConnectionStatus { get; set; } = string.Empty;

    public void Normalize()
    {
        ApiUrl = string.IsNullOrWhiteSpace(ApiUrl) ? DefaultApiUrlValue : ApiUrl.Trim();
        ApiToken = ApiToken?.Trim() ?? string.Empty;
        PollIntervalSeconds = Math.Clamp(PollIntervalSeconds, 1, 3600);
        LanguagePreference = HelperLocalizationService.NormalizeLanguage(LanguagePreference);
        LastConnectionStatus ??= string.Empty;
    }
}
