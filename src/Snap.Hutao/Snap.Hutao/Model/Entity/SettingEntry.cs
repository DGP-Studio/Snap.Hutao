// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

/// <summary>
/// 设置入口
/// </summary>
[HighQuality]
[Table("settings")]
[SuppressMessage("", "SA1124")]
internal sealed partial class SettingEntry
{
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