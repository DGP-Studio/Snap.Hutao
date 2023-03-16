﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.UI;
using System.Runtime.InteropServices;

namespace Snap.Hutao.ViewModel.Achievement;

/// <summary>
/// 成就完成进度更新器
/// </summary>
[HighQuality]
internal sealed class AchievementFinishPercentUpdater
{
    private readonly AchievementViewModel viewModel;

    /// <summary>
    /// 构造一个新的成就进度更新器
    /// </summary>
    /// <param name="viewModel">视图模型</param>
    public AchievementFinishPercentUpdater(AchievementViewModel viewModel)
    {
        this.viewModel = viewModel;
    }

    /// <summary>
    /// 更新完成进度
    /// </summary>
    public void Update()
    {
        int finished = 0;
        int count = 0;

        if (viewModel.Achievements is AdvancedCollectionView achievements)
        {
            if (viewModel.AchievementGoals is List<AchievementGoalView> achievementGoals)
            {
                Dictionary<int, AchievementGoalStatistics> counter = achievementGoals.ToDictionary(x => x.Id, x => new AchievementGoalStatistics(x));
                foreach (AchievementView achievement in achievements.SourceCollection.Cast<AchievementView>())
                {
                    // We want to make the state update as fast as possible,
                    // so we use CollectionsMarshal here to get the ref.
                    ref AchievementGoalStatistics stat = ref CollectionsMarshal.GetValueRefOrNullRef(counter, achievement.Inner.Goal);

                    stat.TotalCount += 1;
                    count += 1;
                    if (achievement.IsChecked)
                    {
                        stat.Finished += 1;
                        finished += 1;
                    }
                }

                foreach (AchievementGoalStatistics statistics in counter.Values)
                {
                    statistics.AchievementGoal.UpdateFinishPercent(statistics.Finished, statistics.TotalCount);
                }

                viewModel.FinishDescription = $"{finished}/{count} - {(double)finished / count:P2}";
            }
        }
    }
}