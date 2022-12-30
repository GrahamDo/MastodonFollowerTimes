using System;
using System.IO;
using Newtonsoft.Json;

namespace MastodonFollowerTimes;

internal class WpfSettings
{
    private const string SettingsFileName = "settings.json";

    public string InstanceUrl { get; set; } = null!;
    public string Token { get; set; } = null!;
    public string AccountName { get; set; } = null!;

    public static WpfSettings Load()
    {
        if (!File.Exists(SettingsFileName))
            return new WpfSettings();

        var text = File.ReadAllText(SettingsFileName);
        return JsonConvert.DeserializeObject<WpfSettings>(text) ??
               throw new ApplicationException($"Your '{SettingsFileName}' appears to be empty or corrupt.");
    }

    public void Save()
    {
        var serialised = JsonConvert.SerializeObject(this);
        File.WriteAllText(SettingsFileName, serialised);
    }
}