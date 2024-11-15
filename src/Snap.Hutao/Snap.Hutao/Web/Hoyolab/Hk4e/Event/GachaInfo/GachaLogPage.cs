// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

internal sealed class GachaLogPage : IJsonOnDeserialized
{
    [JsonPropertyName("page")]
    public string Page { get; set; } = default!;

    [JsonPropertyName("size")]
    public string Size { get; set; } = default!;

    [Obsolete("总是为 0")]
    [JsonPropertyName("total")]
    public string Total { get; set; } = default!;

    [JsonPropertyName("list")]
    public List<GachaLogItem> List { get; set; } = default!;

    [JsonPropertyName("region")]
    [JsonConverter(typeof(RegionConverter))]
    public Region Region { get; set; } = default!;

    public void OnDeserialized()
    {
        // Adjust items timezone
        TimeSpan offset = PlayerUid.GetRegionTimeZoneUtcOffsetForRegion(Region);

        foreach (GachaLogItem item in List)
        {
            item.Time = UnsafeDateTimeOffset.AdjustOffsetOnly(item.Time, offset);
        }
    }
}
