// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Frozen;

namespace Snap.Hutao.Model.InterChange.Achievement;

/// <summary>
/// 统一可交换成就格式
/// https://uigf.org/standards/UIAF.html
/// </summary>
[HighQuality]
internal sealed class UIAF
{
    /// <summary>
    /// 当前发行的版本
    /// </summary>
    public const string CurrentVersion = "v1.1";

    private static readonly FrozenSet<string> SupportedVersion = FrozenSet.ToFrozenSet([CurrentVersion]);

    /// <summary>
    /// 信息
    /// </summary>
    [JsonRequired]
    [JsonPropertyName("info")]
    public UIAFInfo Info { get; set; } = default!;

    /// <summary>
    /// 列表
    /// </summary>
    [JsonPropertyName("list")]
    public List<UIAFItem> List { get; set; } = default!;

    /// <summary>
    /// 确认当前UIAF对象的版本是否受支持
    /// </summary>
    /// <returns>当前UIAF对象是否受支持</returns>
    public bool IsCurrentVersionSupported()
    {
        return SupportedVersion.Contains(Info?.UIAFVersion ?? string.Empty);
    }
}