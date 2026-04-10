using System.Text.Json;

namespace EasyBluetooth.Aida64Helper.Services;

internal sealed class HelperConfigStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _configPath;

    public HelperConfigStore()
    {
        string basePath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "EasyBluetooth",
            "Aida64Helper");
        _configPath = Path.Combine(basePath, "settings.json");
    }

    public Aida64HelperConfig Load()
    {
        try
        {
            if (!File.Exists(_configPath))
            {
                return CreateDefault();
            }

            string json = File.ReadAllText(_configPath);
            var config = JsonSerializer.Deserialize<Aida64HelperConfig>(json) ?? CreateDefault();
            config.Normalize();
            return config;
        }
        catch
        {
            return CreateDefault();
        }
    }

    public void Save(Aida64HelperConfig config)
    {
        ArgumentNullException.ThrowIfNull(config);
        config.Normalize();

        string? directory = Path.GetDirectoryName(_configPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string json = JsonSerializer.Serialize(config, JsonOptions);
        File.WriteAllText(_configPath, json);
    }

    private static Aida64HelperConfig CreateDefault()
    {
        var config = new Aida64HelperConfig();
        config.Normalize();
        return config;
    }
}
