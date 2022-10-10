// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Binding.AvatarProperty;

/// <summary>
/// 圣遗物副词条
/// </summary>
public class ReliquarySubProperty
{
    /// <summary>
    /// 构造副属性
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="value">值</param>
    /// <param name="score">评分</param>
    public ReliquarySubProperty(string name, string value, double score)
    {
        Name = name;
        Value = value;
        Score = score;

        // only 0.2 | 0.4 | 0.6 | 0.8 | 1.0
        Opacity = score switch
        {
            < 25 => 0.25,
            < 50 => 0.5,
            < 75 => 0.75,
            _ => 1,
        };
    }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 值
    /// </summary>
    public string Value { get; }

    /// <summary>
    /// 评分
    /// </summary>
    public double Score { get; }

    /// <summary>
    /// 透明度
    /// </summary>
    public double Opacity { get; }
}
