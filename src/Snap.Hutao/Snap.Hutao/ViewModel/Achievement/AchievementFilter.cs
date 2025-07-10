// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using Snap.Hutao.UI.Xaml.Data;

namespace Snap.Hutao.ViewModel.Achievement;

internal static class AchievementFilter
{
    public static Predicate<AchievementView>? Compile(bool filterDailyQuest, AchievementGoalView? goal)
    {
        return view => DailyQuestComponent(view, filterDailyQuest) && GoalComponent(view, goal);
    }

    public static Predicate<AchievementView>? CompileForAchievementId(bool filterDailyQuest, AchievementGoalView? goal, AchievementId achievementId)
    {
        return view => DailyQuestComponent(view, filterDailyQuest) && GoalComponent(view, goal) && AchievementIdComponent(view, achievementId);
    }

    public static Predicate<AchievementView>? CompileForVersion(bool filterDailyQuest, AchievementGoalView? goal, string version)
    {
        return view => DailyQuestComponent(view, filterDailyQuest) && GoalComponent(view, goal) && VersionComponent(view, version);
    }

    public static Predicate<AchievementView>? CompileForTitleOrDescription(bool filterDailyQuest, AchievementGoalView? goal, string search)
    {
        return view => DailyQuestComponent(view, filterDailyQuest) && GoalComponent(view, goal) && SearchComponent(view, search);
    }

    public static Predicate<AchievementGoalView>? GoalCompile(IAdvancedCollectionView<AchievementView> collection)
    {
        if (collection.Filter is null)
        {
            return default;
        }

        return goal => collection.View.Any(view => view.Inner.Goal == goal.Id);
    }

    private static bool DailyQuestComponent(AchievementView view, bool filterDailyQuest)
    {
        return !filterDailyQuest || view.Inner.IsDailyQuest;
    }

    private static bool GoalComponent(AchievementView view, AchievementGoalView? goal)
    {
        return goal is null || view.Inner.Goal == goal.Id;
    }

    private static bool AchievementIdComponent(AchievementView view, AchievementId achievementId)
    {
        return view.Inner.Id == achievementId;
    }

    private static bool VersionComponent(AchievementView view, string version)
    {
        return string.Equals(view.Inner.Version, version, StringComparison.OrdinalIgnoreCase);
    }

    private static bool SearchComponent(AchievementView view, string search)
    {
        return view.Inner.Title.Contains(search, StringComparison.CurrentCultureIgnoreCase)
            || view.Inner.Description.Contains(search, StringComparison.CurrentCultureIgnoreCase);
    }
}