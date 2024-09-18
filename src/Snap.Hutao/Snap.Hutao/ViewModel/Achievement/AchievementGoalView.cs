// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Metadata.Achievement;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.UI.Xaml.Data;

namespace Snap.Hutao.ViewModel.Achievement;

internal sealed partial class AchievementGoalView : ObservableObject,
    INameIcon,
    IAdvancedCollectionViewItem
{
    private double finishPercent;
    private string? finishDescription;

    private AchievementGoalView(AchievementGoal goal)
    {
        Id = goal.Id;
        Order = goal.Order;
        Name = goal.Name;
        Icon = AchievementIconConverter.IconNameToUri(goal.Icon);
    }

    public AchievementGoalId Id { get; }

    public uint Order { get; }

    public string Name { get; }

    public Uri Icon { get; }

    public double FinishPercent { get => finishPercent; set => SetProperty(ref finishPercent, value); }

    public string? FinishDescription { get => finishDescription; set => SetProperty(ref finishDescription, value); }

    public static AchievementGoalView From(AchievementGoal source)
    {
        return new(source);
    }

    public void UpdateFinishDescriptionAndPercent(AchievementGoalStatistics statistics)
    {
        FinishDescription = AchievementStatistics.Format(statistics.Finished, statistics.TotalCount, out double finishPercent);
        FinishPercent = finishPercent;
    }
}