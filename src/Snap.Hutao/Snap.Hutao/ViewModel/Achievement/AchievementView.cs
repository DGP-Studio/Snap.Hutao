﻿// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Model;
using Snap.Hutao.Model.Intrinsic;

namespace Snap.Hutao.ViewModel.Achievement;

/// <summary>
/// 用于视图绑定的成就
/// </summary>
[HighQuality]
internal sealed class AchievementView : ObservableObject, IEntityWithMetadata<Model.Entity.Achievement, Model.Metadata.Achievement.Achievement>
{
    /// <summary>
    /// 满进度占位符
    /// </summary>
    public const int FullProgressPlaceholder = int.MaxValue;

    private bool isChecked;

    /// <summary>
    /// 构造一个新的成就
    /// </summary>
    /// <param name="entity">实体部分</param>
    /// <param name="inner">元数据部分</param>
    public AchievementView(Model.Entity.Achievement entity, Model.Metadata.Achievement.Achievement inner)
    {
        Entity = entity;
        Inner = inner;

        isChecked = entity.Status >= AchievementStatus.STATUS_FINISHED;
    }

    /// <summary>
    /// 实体
    /// </summary>
    public Model.Entity.Achievement Entity { get; }

    /// <summary>
    /// 元数据
    /// </summary>
    public Model.Metadata.Achievement.Achievement Inner { get; }

    /// <summary>
    /// 是否选中
    /// </summary>
    public bool IsChecked
    {
        get => isChecked;
        set
        {
            if (SetProperty(ref isChecked, value))
            {
                (Entity.Status, Entity.Time) = value
                    ? (AchievementStatus.STATUS_REWARD_TAKEN, DateTimeOffset.UtcNow)
                    : (AchievementStatus.STATUS_INVALID, default);

                OnPropertyChanged(nameof(Time));
            }
        }
    }

    /// <summary>
    /// 格式化的时间
    /// </summary>
    public string Time
    {
        get => $"{Entity.Time.ToLocalTime():yyyy.MM.dd HH:mm:ss}";
    }
}