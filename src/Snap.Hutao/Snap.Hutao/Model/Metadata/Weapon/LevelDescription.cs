// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Metadata.Weapon;

/// <summary>
/// 等级与描述
/// </summary>
/// <typeparam name="TLevel">等级的类型</typeparam>
[HighQuality]
internal sealed class LevelDescription
{
    /// <summary>
    /// 等级
    /// </summary>
    public int Level { get; set; } = default!;

    /// <summary>
    /// 格式化的等级
    /// </summary>
    [JsonIgnore]
    public string LevelFormatted { get => SH.ModelWeaponAffixFormat.Format(Level + 1); }

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; } = default!;
}