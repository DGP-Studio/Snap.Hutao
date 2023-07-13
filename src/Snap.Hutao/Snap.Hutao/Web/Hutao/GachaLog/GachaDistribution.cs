// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.GachaLog;

/// <summary>
/// 祈愿分布
/// </summary>
internal sealed class GachaDistribution
{
    /// <summary>
    /// 总有效抽数
    /// </summary>
    public long TotalValidPulls { get; set; }

    /// <summary>
    /// 分布
    /// </summary>
    public List<PullCount> Distribution { get; set; } = default!;
}