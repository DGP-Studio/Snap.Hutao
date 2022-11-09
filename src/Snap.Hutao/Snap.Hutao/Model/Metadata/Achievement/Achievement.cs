// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Metadata.Achievement;

/// <summary>
/// 成就
/// </summary>
public class Achievement
{
    /// <summary>
    /// Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 分类Id
    /// </summary>
    public int Goal { get; set; }

    /// <summary>
    /// 排序顺序
    /// </summary>
    public int Order { get; set; }

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
    public int Progress { get; set; }

    /// <summary>
    /// 图标
    /// </summary>
    public string? Icon { get; set; }
}