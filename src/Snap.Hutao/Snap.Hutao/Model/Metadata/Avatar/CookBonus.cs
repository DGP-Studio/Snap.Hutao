// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.Model.Metadata.Avatar;

/// <summary>
/// 料理奖励
/// </summary>
public class CookBonus
{
    /// <summary>
    /// 原型名称
    /// </summary>
    public string OriginName { get; set; } = default!;

    /// <summary>
    /// 原型描述
    /// </summary>
    public string OriginDescription { get; set; } = default!;

    /// <summary>
    /// 原型图标
    /// </summary>
    public string OriginIcon { get; set; } = default!;

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; } = default!;

    /// <summary>
    /// 效果描述
    /// </summary>
    public string EffectDescription { get; set; } = default!;

    /// <summary>
    /// 图标
    /// </summary>
    public string Icon { get; set; } = default!;

    /// <summary>
    /// 物品等级
    /// </summary>
    public ItemQuality RankLevel { get; set; }

    /// <summary>
    /// 材料列表
    /// </summary>
    public List<ItemWithCount> InputList { get; set; } = default!;
}