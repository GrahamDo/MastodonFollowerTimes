using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace MastodonFollowerTimes;

internal class MastodonStatus
{
    [JsonProperty("created_at")] 
    public DateTime CreateAtUtc { get; set; }
}