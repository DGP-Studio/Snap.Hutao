// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Enka.Model;

internal sealed class EnkaResponse
{
    [JsonPropertyName("playerInfo")]
    public PlayerInfo? PlayerInfo { get; set; }

    [JsonPropertyName("ttl")]
    public int? TimeToLive { get; set; }

    [JsonIgnore]
    public string Message { get; set; } = default!;
}