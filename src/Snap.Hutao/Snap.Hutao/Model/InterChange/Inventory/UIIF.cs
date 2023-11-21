﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Frozen;

namespace Snap.Hutao.Model.InterChange.Inventory;

/// <summary>
/// 统一可交换物品格式
/// </summary>
[HighQuality]
internal sealed class UIIF
{
    /// <summary>
    /// 当前发行的版本
    /// </summary>
    public const string CurrentVersion = "v1.0";

    private static readonly FrozenSet<string> SupportedVersion = FrozenSet.ToFrozenSet([CurrentVersion]);

    /// <summary>
    /// 信息
    /// </summary>
    [JsonPropertyName("info")]
    public UIIFInfo Info { get; set; } = default!;

    /// <summary>
    /// 列表
    /// </summary>
    [JsonPropertyName("list")]
    public List<UIIFItem> List { get; set; } = default!;

    /// <summary>
    /// 确认当前UIIF对象的版本是否受支持
    /// </summary>
    /// <returns>当前UIIF对象是否受支持</returns>
    public bool IsCurrentVersionSupported()
    {
        return SupportedVersion.Contains(Info?.UIIFVersion ?? string.Empty);
    }
}