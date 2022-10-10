// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Binding.AvatarProperty;

/// <summary>
/// 词条评分
/// </summary>
public struct AffixScore
{
    /// <summary>
    /// 构造一个新的圣遗物评分
    /// </summary>
    /// <param name="score">评分</param>
    /// <param name="weight">最大值</param>
    public AffixScore(double score, double weight)
    {
        Score = score;
        Weight = weight;
    }

    /// <summary>
    /// 评分
    /// </summary>
    public double Score { get; }

    /// <summary>
    /// 权重
    /// </summary>
    public double Weight { get; }
}