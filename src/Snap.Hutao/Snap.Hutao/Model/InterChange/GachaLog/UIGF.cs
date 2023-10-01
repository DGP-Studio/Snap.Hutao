// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.InterChange.GachaLog;

/// <summary>
/// 统一可交换祈愿格式
/// https://uigf.org/standards/UIGF.html
/// </summary>
[HighQuality]
internal sealed class UIGF
{
    /// <summary>
    /// 当前版本
    /// </summary>
    public const string CurrentVersion = "v2.3";

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

    /// <summary>
    /// 确认当前UIGF对象的版本是否受支持
    /// </summary>
    /// <param name="version">版本</param>
    /// <returns>当前UIAF对象是否受支持</returns>
    public bool IsCurrentVersionSupported(out UIGFVersion version)
    {
        version = Info.UIGFVersion switch
        {
            "v2.1" => UIGFVersion.Major2Minor2OrLower,
            "v2.2" => UIGFVersion.Major2Minor2OrLower,
            "v2.3" => UIGFVersion.Major2Minor3OrHigher,
            _ => UIGFVersion.NotSupported,
        };

        return version != UIGFVersion.NotSupported;
    }

    /// <summary>
    /// 列表物品是否正常
    /// </summary>
    /// <param name="itemId">首个出错的Id</param>
    /// <returns>是否正常</returns>
    public bool IsMajor2Minor2OrLowerListValid([NotNullWhen(false)] out long itemId)
    {
        foreach (UIGFItem item in List)
        {
            if (item.ItemType != SH.ModelInterchangeUIGFItemTypeAvatar && item.ItemType != SH.ModelInterchangeUIGFItemTypeWeapon)
            {
                itemId = item.Id;
                return false;
            }
        }

        itemId = 0;
        return true;
    }
}