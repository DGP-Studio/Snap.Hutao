// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using BindingAchievementGoal = Snap.Hutao.ViewModel.Achievement.AchievementGoalView;

namespace Snap.Hutao.ViewModel.Achievement;

internal struct AchievementGoalStatistics
{
    public readonly BindingAchievementGoal AchievementGoal;
    public int Finished;
    public int TotalCount;

    private AchievementGoalStatistics(BindingAchievementGoal goal)
    {
        AchievementGoal = goal;
    }

    public static AchievementGoalStatistics From(BindingAchievementGoal goal)
    {
        return new(goal);
    }
}