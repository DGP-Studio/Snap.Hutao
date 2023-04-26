// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Achievement;

/// <summary>
/// 成就分类
/// </summary>
[HighQuality]
internal sealed class AchievementGoal
{
    /// <summary>
    /// Id
    /// </summary>
    public AchievementGoalId Id { get; set; }

    /// <summary>
    /// 排序顺序
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// 完成奖励
    /// </summary>
    public Reward? FinishReward { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    public string Icon { get; set; } = default!;
}