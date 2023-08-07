// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Metadata.Achievement;
using Snap.Hutao.Model.Metadata.Converter;
using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.ViewModel.Achievement;

/// <summary>
/// 绑定成就分类
/// </summary>
[HighQuality]
internal sealed class AchievementGoalView : ObservableObject, INameIcon, IMappingFrom<AchievementGoalView, AchievementGoal>
{
    private double finishPercent;
    private string? finishDescription;

    /// <summary>
    /// 构造一个新的成就分类
    /// </summary>
    /// <param name="goal">分类</param>
    private AchievementGoalView(AchievementGoal goal)
    {
        Id = goal.Id;
        Order = goal.Order;
        Name = goal.Name;
        Icon = AchievementIconConverter.IconNameToUri(goal.Icon);
    }

    /// <summary>
    /// Id
    /// </summary>
    public AchievementGoalId Id { get; }

    /// <summary>
    /// 排序顺序
    /// </summary>
    public uint Order { get; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// 图标
    /// </summary>
    public Uri Icon { get; }

    /// <summary>
    /// 完成百分比
    /// </summary>
    public double FinishPercent { get => finishPercent; set => SetProperty(ref finishPercent, value); }

    /// <summary>
    /// 完成百分比描述
    /// </summary>
    public string? FinishDescription { get => finishDescription; set => SetProperty(ref finishDescription, value); }

    public static AchievementGoalView From(AchievementGoal source)
    {
        return new(source);
    }

    /// <summary>
    /// 更新进度
    /// </summary>
    /// <param name="statistics">统计</param>
    public void UpdateFinishDescriptionAndPercent(AchievementGoalStatistics statistics)
    {
        UpdateFinishDescriptionAndPercent(statistics.Finished, statistics.TotalCount);
    }

    /// <summary>
    /// 更新进度
    /// </summary>
    /// <param name="finished">完成项</param>
    /// <param name="count">总项</param>
    private void UpdateFinishDescriptionAndPercent(int finished, int count)
    {
        FinishDescription = AchievementStatistics.Format(finished, count, out double finishPercent);
        FinishPercent = finishPercent;
    }
}