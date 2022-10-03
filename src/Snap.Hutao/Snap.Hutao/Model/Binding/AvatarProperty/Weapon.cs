// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Binding.AvatarProperty;

/// <summary>
/// 武器
/// </summary>
public class Weapon : EquipBase
{
    /// <summary>
    /// 副属性
    /// </summary>
    public Pair<string, string> SubProperty { get; set; } = default!;

    /// <summary>
    /// 精炼属性
    /// </summary>
    public int AffixLevel { get; set; }

    /// <summary>
    /// 精炼名称
    /// </summary>
    public string AffixName { get; set; } = default!;

    /// <summary>
    /// 精炼被动
    /// </summary>
    public string AffixDescription { get; set; } = default!;
}