// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using BindingAchievementGoal = Snap.Hutao.Model.Binding.Achievement.AchievementGoal;

namespace Snap.Hutao.ViewModel.Achievement;

/// <summary>
/// 成就分类统计
/// </summary>
internal struct AchievementGoalStatistics
{
    /// <summary>
    /// 成就分类
    /// </summary>
    public readonly BindingAchievementGoal AchievementGoal;

    /// <summary>
    /// 完成数
    /// </summary>
    public int Finished;

    /// <summary>
    /// 总数
    /// </summary>
    public int TotalCount;

    /// <summary>
    /// 构造一个新的成就分类统计
    /// </summary>
    /// <param name="goal">分类</param>
    public AchievementGoalStatistics(BindingAchievementGoal goal)
    {
        AchievementGoal = goal;
    }
}