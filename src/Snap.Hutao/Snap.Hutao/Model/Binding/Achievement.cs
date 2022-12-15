// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;

namespace Snap.Hutao.Model.Binding;

/// <summary>
/// 用于视图绑定的成就
/// </summary>
public class Achievement : ObservableObject
{
    /// <summary>
    /// 满进度占位符
    /// </summary>
    public const int FullProgressPlaceholder = int.MaxValue;

    private bool isChecked;

    /// <summary>
    /// 构造一个新的成就
    /// </summary>
    /// <param name="inner">元数据部分</param>
    /// <param name="entity">实体部分</param>
    public Achievement(Metadata.Achievement.Achievement inner, Entity.Achievement entity)
    {
        Inner = inner;
        Entity = entity;

        isChecked = (int)entity.Status >= 2;
    }

    /// <summary>
    /// 实体
    /// </summary>
    public Entity.Achievement Entity { get; }

    /// <summary>
    /// 元数据
    /// </summary>
    public Metadata.Achievement.Achievement Inner { get; }

    /// <summary>
    /// 是否选中
    /// </summary>
    public bool IsChecked
    {
        get => isChecked;
        set
        {
            SetProperty(ref isChecked, value);

            // Only update state when checked
            if (value)
            {
                Entity.Status = Intrinsic.AchievementInfoStatus.STATUS_REWARD_TAKEN;
                Entity.Time = DateTimeOffset.Now;
                OnPropertyChanged(nameof(Time));
            }
        }
    }

    /// <summary>
    /// 格式化的时间
    /// </summary>
    public string Time
    {
        get => Entity.Time.ToString("yyyy.MM.dd HH:mm:ss");
    }
}