// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.ViewModel.Achievement;

/// <summary>
/// 成就统计
/// </summary>
internal sealed class AchievementStatistics
{
    /// <summary>
    /// 存档显示名称
    /// </summary>
    public string DisplayName { get; set; } = default!;

    /// <summary>
    /// 完成进度描述 xxx/yyy
    /// </summary>
    public string FinishDescription { get; set; } = default!;

    /// <summary>
    /// 近期完成的成就
    /// </summary>
    public List<AchievementView> Achievements { get; set; } = default!;
}