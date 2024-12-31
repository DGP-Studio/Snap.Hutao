// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.ExceptionService;
using Snap.Hutao.Model.Primitive;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Snap.Hutao.ViewModel.Achievement;

internal static class AchievementFinishPercent
{
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

        if (achievements.SourceCollection is not ImmutableArray<AchievementView> array)
        {
            throw HutaoException.InvalidCast<IList<AchievementView>, ImmutableArray<AchievementView>>("AchievementViewModel.Achievements.SourceCollection");
        }

        Dictionary<AchievementGoalId, AchievementGoalStatistics> counter = achievementGoals.SourceCollection.ToDictionary(x => x.Id, AchievementGoalStatistics.From);

        foreach (ref readonly AchievementView achievementView in array.AsSpan())
        {
            ref AchievementGoalStatistics goalStat = ref CollectionsMarshal.GetValueRefOrNullRef(counter, achievementView.Inner.Goal);
            Debug.Assert(!Unsafe.IsNullRef(in goalStat));

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