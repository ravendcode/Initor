using System;
using System.IO;
using System.Threading.Tasks;
using AvaloniaInitor.Helpers;
using AvaloniaInitor.Models;
using dotenv.net;
using YamlDotNet.Serialization;

namespace AvaloniaInitor.Data;

public interface IAppUserSettingsDataProvider
{
    Task<AppUserSettings?> LoadAsync();
    void Store(AppUserSettings appUserSettings);
}

public class AppUserSettingsDataProvider : IAppUserSettingsDataProvider
{
    // private const string AppUserSettingsFile = "Settings\\AppUserSettings.yaml";
    private readonly string _appUserSettingsFile = DotEnv.Read()["APP_USER_SETTINGS_FILE"];

    public async Task<AppUserSettings?> LoadAsync()
    {
        try
        {
            using var reader = new StreamReader(_appUserSettingsFile);
            var yml = await reader.ReadToEndAsync();
            var deserializer = new DeserializerBuilder().Build();
            return deserializer.Deserialize<AppUserSettings>(yml);
        }
        catch (Exception e)
        {
            await MessageBoxHelper.Error(e.Message);
        }

        return null;
    }

    public void Store(AppUserSettings appUserSettings)
    {
        var serializer = new SerializerBuilder().Build();
        var yaml = serializer.Serialize(appUserSettings);
        File.WriteAllText(_appUserSettingsFile, yaml);
    }
}
