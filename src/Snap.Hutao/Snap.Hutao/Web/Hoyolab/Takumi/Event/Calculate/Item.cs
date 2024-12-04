// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.Calculate;

internal sealed class Item
{
    [JsonPropertyName("id")]
    public uint Id { get; set; }

    [JsonPropertyName("name")]
    public string Name { get; set; } = default!;

    [JsonPropertyName("icon_url")]
    public Uri Icon { get; set; } = default!;

    [JsonPropertyName("num")]
    public uint Num { get; set; }

    [JsonPropertyName("level")]
    public QualityType Level { get; set; }

    [JsonPropertyName("lack_num")]
    public int LackNum { get; set; }
}