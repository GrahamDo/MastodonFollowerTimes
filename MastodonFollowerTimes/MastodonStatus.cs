using System;
using System.Text.Json.Serialization;

namespace MastodonFollowerTimes;

internal class MastodonStatus    
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
    [JsonPropertyName("created_at")]
    public DateTime CreateAtUtc { get; set; }
}