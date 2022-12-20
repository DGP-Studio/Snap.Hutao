// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;

namespace Snap.Hutao.Model.Binding.Achievement;

/// <summary>
/// 绑定成就分类
/// </summary>
public class AchievementGoal : ObservableObject
{
    private double finishPercent;
    private string? finishDescription;

    /// <summary>
    /// 构造一个新的成就分类
    /// </summary>
    /// <param name="goal">分类</param>
    public AchievementGoal(Metadata.Achievement.AchievementGoal goal)
    {
        Id = goal.Id;
        Order = goal.Order;
        Name = goal.Name;
        Icon = Metadata.Converter.AchievementIconConverter.IconNameToUri(goal.Icon);
    }

    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 排序顺序
    /// </summary>
    public int Order { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// 图标
    /// </summary>
    public Uri? Icon { get; set; }

    /// <summary>
    /// 完成百分比
    /// </summary>
    public double FinishPercent { get => finishPercent; set => SetProperty(ref finishPercent, value); }

    /// <summary>
    /// 完成百分比
    /// </summary>
    public string? FinishDescription { get => finishDescription; set => SetProperty(ref finishDescription, value); }

    /// <summary>
    /// 更新进度
    /// </summary>
    /// <param name="finished">完成项</param>
    /// <param name="count">总项</param>
    public void UpdateFinishPercent(int finished, int count)
    {
        FinishPercent = (double)finished / count;
        FinishDescription = $"{finished}/{count} - {FinishPercent:P2}";
    }
}