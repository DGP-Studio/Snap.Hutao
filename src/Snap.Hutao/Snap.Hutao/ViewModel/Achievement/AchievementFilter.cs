// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using Snap.Hutao.UI.Xaml.Data;

namespace Snap.Hutao.ViewModel.Achievement;

internal static class AchievementFilter
{
    public static Predicate<AchievementView>? Compile(bool filterDailyQuest)
    {
        return filterDailyQuest
            ? view => view.Inner.IsDailyQuest
            : default;
    }

    public static Predicate<AchievementView>? Compile(bool filterDailyQuest, AchievementGoalView? goal)
    {
        if (goal is null)
        {
            return Compile(filterDailyQuest);
        }

        return filterDailyQuest
            ? view => view.Inner.IsDailyQuest && view.Inner.Goal == goal.Id
            : view => view.Inner.Goal == goal.Id;
    }

    public static Predicate<AchievementView>? Compile(bool filterDailyQuest, AchievementId achievementId)
    {
        return filterDailyQuest
            ? view => view.Inner.IsDailyQuest && view.Inner.Id == achievementId
            : view => view.Inner.Id == achievementId;
    }

    public static Predicate<AchievementView>? CompileForVersion(bool filterDailyQuest, string version)
    {
        return filterDailyQuest
            ? view => view.Inner.IsDailyQuest && string.Equals(view.Inner.Version, version, StringComparison.CurrentCultureIgnoreCase)
            : view => string.Equals(view.Inner.Version, version, StringComparison.CurrentCultureIgnoreCase);
    }

    public static Predicate<AchievementView>? CompileForTitleOrDescription(bool filterDailyQuest, string search)
    {
        return filterDailyQuest
            ? view => view.Inner.IsDailyQuest && (view.Inner.Title.Contains(search, StringComparison.CurrentCultureIgnoreCase) || view.Inner.Description.Contains(search, StringComparison.CurrentCultureIgnoreCase))
            : view => view.Inner.Title.Contains(search, StringComparison.CurrentCultureIgnoreCase) || view.Inner.Description.Contains(search, StringComparison.CurrentCultureIgnoreCase);
    }

    public static Predicate<AchievementGoalView>? GoalCompile(AdvancedCollectionView<AchievementView> collection)
    {
        if (collection.Filter is null)
        {
            return default;
        }

        return goal => collection.View.Any(view => view.Inner.Goal == goal.Id);
    }
}