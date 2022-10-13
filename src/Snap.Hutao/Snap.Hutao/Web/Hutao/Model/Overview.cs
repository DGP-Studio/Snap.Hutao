// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.Model;

/// <summary>
/// 统计数据
/// </summary>
public class Overview
{
    /// <summary>
    /// 规划Id
    /// </summary>
    public int ScheduleId { get; set; }

    /// <summary>
    /// 总记录数
    /// </summary>
    public int RecordTotal { get; set; }

    /// <summary>
    /// 总深渊计数
    /// </summary>
    public int SpiralAbyssTotal { get; set; }

    /// <summary>
    /// 满星数
    /// </summary>
    public int SpiralAbyssFullStar { get; set; }
}