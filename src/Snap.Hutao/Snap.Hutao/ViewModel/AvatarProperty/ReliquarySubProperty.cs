// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.ViewModel.AvatarProperty;

/// <summary>
/// 圣遗物副词条
/// </summary>
[HighQuality]
internal class ReliquarySubProperty
{
    public ReliquarySubProperty(FightProperty type, string value, float score)
    {
        Name = type.GetLocalizedDescription();
        Value = value;
        Score = score;

        // only 0.25 | 0.50 | 0.75 | 1.00
        Opacity = score == 0 ? 0.25 : Math.Ceiling(score / 25) / 4;
    }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 属性值
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// 透明度
    /// </summary>
    public double Opacity { get; }

    /// <summary>
    /// 评分
    /// </summary>
    internal float Score { get; }
}