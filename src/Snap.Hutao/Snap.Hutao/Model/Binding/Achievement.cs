// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Binding;

/// <summary>
/// 用于视图绑定的成就
/// </summary>
public class Achievement : Observable
{
    /// <summary>
    /// 满进度占位符
    /// </summary>
    public const int FullProgressPlaceholder = int.MaxValue;

    private readonly Metadata.Achievement.Achievement inner;
    private readonly Entity.Achievement entity;

    private bool isChecked;

    /// <summary>
    /// 构造一个新的成就
    /// </summary>
    /// <param name="inner">元数据部分</param>
    /// <param name="entity">实体部分</param>
    public Achievement(Metadata.Achievement.Achievement inner, Entity.Achievement entity)
    {
        this.inner = inner;
        this.entity = entity;

        // Property should only be set when is  user checking.
        isChecked = (int)entity.Status >= 2;
    }

    /// <summary>
    /// 实体
    /// </summary>
    public Entity.Achievement Entity { get => entity; }

    /// <summary>
    /// 元数据
    /// </summary>
    public Metadata.Achievement.Achievement Inner { get => inner; }

    /// <summary>
    /// 是否选中
    /// </summary>
    public bool IsChecked
    {
        get => isChecked;
        set
        {
            Set(ref isChecked, value);

            // Only update state when checked
            if (value)
            {
                Entity.Status = Intrinsic.AchievementInfoStatus.ACHIEVEMENT_POINT_TAKEN;
                Entity.Time = DateTimeOffset.Now;
            }
        }
    }
}