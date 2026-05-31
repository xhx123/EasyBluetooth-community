using System.Text.Json;

namespace EasyBluetooth.Aida64Helper.Services;

internal sealed class HelperConfigStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _configPath;
    private readonly string _legacyConfigPath;

    public HelperConfigStore()
    {
        string rootPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "EasyBluetooth");
        string basePath = Path.Combine(rootPath, "EasyBluetoothExportHelper");
        _configPath = Path.Combine(basePath, "settings.json");
        _legacyConfigPath = Path.Combine(rootPath, "Aida64Helper", "settings.json");
    }

    public Aida64HelperConfig Load()
    {
        try
        {
            if (!File.Exists(_configPath))
            {
                if (File.Exists(_legacyConfigPath))
                {
                    var migrated = LoadFromPath(_legacyConfigPath);
                    Save(migrated);
                    return migrated;
                }

                return CreateDefault();
            }

            return LoadFromPath(_configPath);
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

    private static Aida64HelperConfig LoadFromPath(string path)
    {
        string json = File.ReadAllText(path);
        var config = JsonSerializer.Deserialize<Aida64HelperConfig>(json) ?? CreateDefault();
        config.Normalize();
        return config;
    }
}
