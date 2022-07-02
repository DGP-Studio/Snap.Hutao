// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Metadata.Achievement;

/// <summary>
/// 成就触发器
/// </summary>
public class AchievementTrigger
{
    /// <summary>
    /// 触发器类型
    /// </summary>
    public AchievementTriggerType Type { get; set; }

    /// <summary>
    /// Id
    /// </summary>
    public string Id { get; set; } = default!;

    /// <summary>
    /// 标题
    /// </summary>
    public string Title { get; set; } = default!;

    /// <summary>
    /// 描述
    /// </summary>
    public string Description { get; set; } = default!;
}
