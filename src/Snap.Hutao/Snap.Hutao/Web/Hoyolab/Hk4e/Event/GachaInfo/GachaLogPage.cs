// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core;

namespace Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;

/// <summary>
/// 祈愿记录分页
/// </summary>
[HighQuality]
internal sealed class GachaLogPage : IJsonOnDeserialized
{
    /// <summary>
    /// 页码
    /// </summary>
    [JsonPropertyName("page")]
    public string Page { get; set; } = default!;

    /// <summary>
    /// 尺寸
    /// </summary>
    [JsonPropertyName("size")]
    public string Size { get; set; } = default!;

    /// <summary>
    /// 总页数
    /// </summary>
    [Obsolete("总是为 0")]
    [JsonPropertyName("total")]
    public string Total { get; set; } = default!;

    /// <summary>
    /// 总页数
    /// 总是为 0
    /// </summary>
    [JsonPropertyName("list")]
    public List<GachaLogItem> List { get; set; } = default!;

    /// <summary>
    /// 地区
    /// </summary>
    [JsonPropertyName("region")]
    public string Region { get; set; } = default!;

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
