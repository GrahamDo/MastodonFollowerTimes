using System;
using System.IO;
using Newtonsoft.Json;

namespace MastodonFollowerTimes;

internal class WpfSettings
{
    private const string SettingsFileName = "settings.json";

    public string InstanceUrl { get; set; }
    public string Token { get; set; }
    public string AccountName { get; set; }

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