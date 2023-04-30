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

    /// <summary>
    /// 格式化完成进度
    /// </summary>
    /// <param name="finished">完成的成就个数</param>
    /// <param name="totalCount">总个数</param>
    /// <param name="finishedPercent">完成进度</param>
    /// <returns>格式化的完成进度</returns>
    public static string Format(int finished, int totalCount, out double finishedPercent)
    {
        finishedPercent = (double)finished / totalCount;
        return $"{finished}/{totalCount} - {finishedPercent:P2}";
    }
}