// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.ViewModel.AvatarProperty;

/// <summary>
/// 圣遗物
/// </summary>
[HighQuality]
internal sealed class ReliquaryView : Equip
{
    /// <summary>
    /// 初始词条
    /// </summary>
    public List<ReliquarySubProperty> PrimarySubProperties { get; set; } = default!;

    /// <summary>
    /// 强化词条
    /// </summary>
    public List<ReliquarySubProperty> SecondarySubProperties { get; set; } = default!;

    /// <summary>
    /// 合成的副属性
    /// </summary>
    public List<ReliquarySubProperty> ComposedSubProperties { get; set; } = default!;

    /// <summary>
    /// 格式化评分
    /// </summary>
    public string ScoreFormatted { get => $"{Score:F2}"; }

    /// <summary>
    /// 评分
    /// </summary>
    internal float Score { get; set; }
}