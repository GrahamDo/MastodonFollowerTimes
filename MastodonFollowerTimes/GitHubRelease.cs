using Newtonsoft.Json;

namespace MastodonFollowerTimes;

internal class GitHubRelease
{
    [JsonProperty("tag_name")]
    public string TagName { get; set; } = null!;

    [JsonProperty("draft")]
    public bool IsDraft { get; set; }
    [JsonProperty("prerelease")]
    public bool IsPreRelease { get; set; }
}