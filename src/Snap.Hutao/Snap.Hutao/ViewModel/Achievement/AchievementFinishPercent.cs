// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
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

        if (viewModel.Achievements is not { } achievements)
        {
            return;
        }

        if (viewModel.AchievementGoals is not { } achievementGoals)
        {
            return;
        }

        if (achievements.SourceCollection is not List<AchievementView> list)
        {
            throw HutaoException.InvalidCast<IEnumerable<AchievementView>, List<AchievementView>>("AchievementViewModel.Achievements.SourceCollection");
        }

        Dictionary<AchievementGoalId, AchievementGoalStatistics> counter = achievementGoals.ToDictionary(x => x.Id, AchievementGoalStatistics.From);

        foreach (ref readonly AchievementView achievementView in CollectionsMarshal.AsSpan(list))
        {
            ref AchievementGoalStatistics goalStat = ref CollectionsMarshal.GetValueRefOrNullRef(counter, achievementView.Inner.Goal);

            goalStat.TotalCount += 1;
            totalCount += 1;
            if (achievementView.IsChecked)
            {
                goalStat.Finished += 1;
                totalFinished += 1;
            }
        }

        foreach (AchievementGoalStatistics statistics in counter.Values)
        {
            statistics.AchievementGoal.UpdateFinishDescriptionAndPercent(statistics);
        }

        viewModel.FinishDescription = AchievementStatistics.Format(totalFinished, totalCount, out _);
    }
}