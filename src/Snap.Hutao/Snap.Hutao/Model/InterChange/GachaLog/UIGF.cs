// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab;
using System.Runtime.InteropServices;

namespace Snap.Hutao.Model.InterChange.GachaLog;

/// <summary>
/// 统一可交换祈愿格式
/// https://uigf.org/standards/UIGF.html
/// </summary>
[HighQuality]
internal sealed class UIGF : IJsonOnSerializing, IJsonOnDeserialized
{
    /// <summary>
    /// 当前版本
    /// </summary>
    public const string CurrentVersion = "v2.4";

    /// <summary>
    /// 信息
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("info")]
    public UIGFInfo Info { get; set; } = default!;

    /// <summary>
    /// 列表
    /// </summary>
    [JsonPropertyName("list")]
    public List<UIGFItem> List { get; set; } = default!;

    public void OnSerializing()
    {
        TimeSpan offset = GetRegionTimeZoneUtcOffset();

        foreach (UIGFItem item in List)
        {
            item.Time = item.Time.ToOffset(offset);
        }
    }

    public void OnDeserialized()
    {
        // Adjust items timezone
        TimeSpan offset = GetRegionTimeZoneUtcOffset();

        foreach (UIGFItem item in List)
        {
            item.Time = UnsafeDateTimeOffset.AdjustOffsetOnly(item.Time, offset);
        }
    }

    public bool IsCurrentVersionSupported(out UIGFVersion version)
    {
        version = Info.UIGFVersion switch
        {
            "v2.1" => UIGFVersion.Major2Minor2OrLower,
            "v2.2" => UIGFVersion.Major2Minor2OrLower,
            "v2.3" => UIGFVersion.Major2Minor3OrHigher,
            "v2.4" => UIGFVersion.Major2Minor3OrHigher,
            _ => UIGFVersion.NotSupported,
        };

        return version != UIGFVersion.NotSupported;
    }

    public bool IsMajor2Minor2OrLowerListValid([NotNullWhen(false)] out long id)
    {
        foreach (ref readonly UIGFItem item in CollectionsMarshal.AsSpan(List))
        {
            if (item.ItemType != SH.ModelInterchangeUIGFItemTypeAvatar && item.ItemType != SH.ModelInterchangeUIGFItemTypeWeapon)
            {
                id = item.Id;
                return false;
            }
        }

        id = 0;
        return true;
    }

    public bool IsMajor2Minor3OrHigherListValid([NotNullWhen(false)] out long id)
    {
        foreach (ref readonly UIGFItem item in CollectionsMarshal.AsSpan(List))
        {
            if (string.IsNullOrEmpty(item.ItemId))
            {
                id = item.Id;
                return false;
            }
        }

        id = 0;
        return true;
    }

    private TimeSpan GetRegionTimeZoneUtcOffset()
    {
        if (Info.RegionTimeZone is int offsetHours)
        {
            return new TimeSpan(offsetHours, 0, 0);
        }

        return PlayerUid.GetRegionTimeZoneUtcOffset(Info.Uid);
    }
}