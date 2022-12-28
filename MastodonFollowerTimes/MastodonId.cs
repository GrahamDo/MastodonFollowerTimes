using System.Text.Json.Serialization;

namespace MastodonFollowerTimes;

internal class MastodonId
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
}