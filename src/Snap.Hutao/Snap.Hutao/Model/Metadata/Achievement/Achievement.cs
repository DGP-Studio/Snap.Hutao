// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Model.Metadata.Achievement;

/// <summary>
/// 成就
/// </summary>
[HighQuality]
internal sealed class Achievement
{
    /// <summary>
    /// Id
    /// </summary>
    public AchievementId Id { get; set; }

    /// <summary>
    /// 分类Id
    /// </summary>
    public AchievementGoalId Goal { get; set; }

    /// <summary>
    /// 排序顺序
    /// </summary>
    public uint Order { get; set; }

    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; } = default!;

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; } = default!;

    /// <summary>
    /// 完成奖励
    /// </summary>
    public Reward FinishReward { get; set; } = default!;

    /// <summary>
    /// 总进度
    /// </summary>
    public uint Progress { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    public string? Icon { get; set; }

    /// <summary>
    /// 版本
    /// </summary>
    public string Version { get; set; } = default!;
}