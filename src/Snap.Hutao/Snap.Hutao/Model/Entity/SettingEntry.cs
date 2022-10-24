// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

/// <summary>
/// 设置入口
/// </summary>
[Table("settings")]
public class SettingEntry
{
    /// <summary>
    /// 游戏路径
    /// </summary>
    public const string GamePath = "GamePath";

    /// <summary>
    /// 空的历史记录卡池是否可见
    /// </summary>
    public const string IsEmptyHistoryWishVisible = "IsEmptyHistoryWishVisible";

    /// <summary>
    /// 构造一个新的设置入口
    /// </summary>
    /// <param name="key">键</param>
    /// <param name="value">值</param>
    public SettingEntry(string key, string? value)
    {
        Key = key;
        Value = value;
    }

    /// <summary>
    /// 键
    /// </summary>
    [Key]
    public string Key { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public string? Value { get; set; }
}
