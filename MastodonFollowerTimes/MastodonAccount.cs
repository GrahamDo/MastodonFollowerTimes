﻿using System.Text.Json.Serialization;

namespace MastodonFollowerTimes;

internal class MastodonAccount
{
    [JsonPropertyName("id")]
    public string Id { get; set; }
}