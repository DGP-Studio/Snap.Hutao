// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.GachaLog;

/// <summary>
/// 抽数与个数
/// </summary>
internal sealed class PullCount
{
    /// <summary>
    /// 抽数
    /// </summary>
    public int Pull { get; set; }

    /// <summary>
    /// 对应个数
    /// </summary>
    public long Count { get; set; }
}