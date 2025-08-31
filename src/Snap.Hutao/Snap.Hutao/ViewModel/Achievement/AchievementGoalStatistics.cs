// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using BindingAchievementGoal = Snap.Hutao.ViewModel.Achievement.AchievementGoalView;

namespace Snap.Hutao.ViewModel.Achievement;

internal sealed class AchievementGoalStatistics
{
    private AchievementGoalStatistics(BindingAchievementGoal goal)
    {
        AchievementGoal = goal;
    }

    public BindingAchievementGoal AchievementGoal { get; }

    public int Finished { get; set; }

    public int TotalCount { get; set; }

    public static AchievementGoalStatistics Create(BindingAchievementGoal goal)
    {
        return new(goal);
    }
}