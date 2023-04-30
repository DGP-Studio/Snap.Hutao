// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.WinUI.UI;
using Snap.Hutao.Model.Primitive;
using System.Runtime.InteropServices;

namespace Snap.Hutao.ViewModel.Achievement;

/// <summary>
/// 成就完成进度
/// </summary>
[HighQuality]
internal static class AchievementFinishPercent
{
    /// <summary>
    /// 更新完成进度
    /// </summary>
    /// <param name="viewModel">视图模型</param>
    public static void Update(AchievementViewModel viewModel)
    {
        int totalFinished = 0;
        int totalCount = 0;

        if (viewModel.Achievements is AdvancedCollectionView achievements)
        {
            if (viewModel.AchievementGoals is List<AchievementGoalView> achievementGoals)
            {
                Dictionary<AchievementGoalId, AchievementGoalStatistics> counter = achievementGoals.ToDictionary(x => x.Id, AchievementGoalStatistics.Create);
                foreach (AchievementView achievement in achievements.SourceCollection.Cast<AchievementView>())
                {
                    // Make the state update as fast as possible
                    ref AchievementGoalStatistics stat = ref CollectionsMarshal.GetValueRefOrNullRef(counter, achievement.Inner.Goal);

                    stat.TotalCount += 1;
                    totalCount += 1;
                    if (achievement.IsChecked)
                    {
                        stat.Finished += 1;
                        totalFinished += 1;
                    }
                }

                foreach (AchievementGoalStatistics statistics in counter.Values)
                {
                    statistics.AchievementGoal.UpdateFinishPercent(statistics);
                }

                viewModel.FinishDescription = AchievementStatistics.Format(totalFinished, totalCount, out _);
            }
        }
    }
}