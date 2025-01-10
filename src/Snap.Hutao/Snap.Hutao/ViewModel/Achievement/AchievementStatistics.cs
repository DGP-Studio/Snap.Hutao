// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.ViewModel.Achievement;

internal sealed class AchievementStatistics
{
    public required string DisplayName { get; init; }

    public required string FinishDescription { get; init; }

    public required ImmutableArray<AchievementView> Achievements { get; init; }

    /// <summary>
    /// 格式化完成进度
    /// "xxx/yyy - z.zz%"
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