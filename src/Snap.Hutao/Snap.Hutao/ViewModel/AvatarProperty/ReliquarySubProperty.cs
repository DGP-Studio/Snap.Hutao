// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.ViewModel.AvatarProperty;

/// <summary>
/// 圣遗物副词条
/// </summary>
[HighQuality]
internal sealed class ReliquarySubProperty
{
    /// <summary>
    /// 构造副属性
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="value">值</param>
    /// <param name="score">评分</param>
    public ReliquarySubProperty(string name, string value, float score)
    {
        Name = name;
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
