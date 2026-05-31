using System.Xml.Linq;

namespace EasyBluetooth.Aida64Helper.Services;

internal static class HelperLocalizationService
{
    private static readonly object Gate = new();
    private static Dictionary<string, string> _current = new(StringComparer.Ordinal);
    private static Dictionary<string, string> _fallback = new(StringComparer.Ordinal);

    public static string CurrentLanguage { get; private set; } = Aida64HelperConfig.DefaultLanguagePreferenceValue;

    public static void Initialize(string? preferredLanguage)
    {
        lock (Gate)
        {
            CurrentLanguage = NormalizeLanguage(preferredLanguage);
            _fallback = LoadResw("en-US");
            _current = string.Equals(CurrentLanguage, "en-US", StringComparison.OrdinalIgnoreCase)
                ? _fallback
                : LoadResw(CurrentLanguage);
        }
    }

    public static string NormalizeLanguage(string? language)
    {
        if (!string.IsNullOrWhiteSpace(language) &&
            language.Trim().StartsWith("zh", StringComparison.OrdinalIgnoreCase))
        {
            return "zh-CN";
        }

        return "en-US";
    }

    public static string GetString(string key, string? fallback = null)
    {
        lock (Gate)
        {
            if (_current.TryGetValue(key, out string? value) && !string.IsNullOrWhiteSpace(value))
            {
                return value;
            }

            if (_fallback.TryGetValue(key, out value) && !string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return fallback ?? key;
    }

    public static string Format(string key, params object[] args)
    {
        return string.Format(GetString(key), args);
    }

    private static Dictionary<string, string> LoadResw(string language)
    {
        var embeddedResources = LoadReswFromEmbeddedResource(language);
        return embeddedResources.Count > 0
            ? embeddedResources
            : LoadReswFromFile(language);
    }

    private static Dictionary<string, string> LoadReswFromEmbeddedResource(string language)
    {
        string suffix = $"Strings.{language}.Resources.resw";
        var assembly = typeof(HelperLocalizationService).Assembly;
        string? resourceName = assembly
            .GetManifestResourceNames()
            .FirstOrDefault(name => name.EndsWith(suffix, StringComparison.OrdinalIgnoreCase));

        if (string.IsNullOrWhiteSpace(resourceName))
        {
            return CreateEmptyResourceMap();
        }

        try
        {
            using Stream? stream = assembly.GetManifestResourceStream(resourceName);
            return stream is null
                ? CreateEmptyResourceMap()
                : LoadReswFromStream(stream);
        }
        catch
        {
            return CreateEmptyResourceMap();
        }
    }

    private static Dictionary<string, string> LoadReswFromFile(string language)
    {
        string path = Path.Combine(AppContext.BaseDirectory, "Strings", language, "Resources.resw");
        if (!File.Exists(path))
        {
            return CreateEmptyResourceMap();
        }

        try
        {
            using var stream = File.OpenRead(path);
            return LoadReswFromStream(stream);
        }
        catch
        {
            return CreateEmptyResourceMap();
        }
    }

    private static Dictionary<string, string> LoadReswFromStream(Stream stream)
    {
        var document = XDocument.Load(stream);
        return document.Root?
            .Elements("data")
            .Select(element => new
            {
                Key = element.Attribute("name")?.Value,
                Value = element.Element("value")?.Value
            })
            .Where(item => !string.IsNullOrWhiteSpace(item.Key))
            .ToDictionary(item => item.Key!, item => item.Value ?? string.Empty, StringComparer.Ordinal)
            ?? CreateEmptyResourceMap();
    }

    private static Dictionary<string, string> CreateEmptyResourceMap()
    {
        return new Dictionary<string, string>(StringComparer.Ordinal);
    }
}
