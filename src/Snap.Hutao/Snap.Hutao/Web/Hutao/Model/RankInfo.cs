// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.Model;

/// <summary>
/// 排行信息
/// </summary>
public class RankInfo
{
    /// <summary>
    /// 角色Id
    /// </summary>
    public int AvatarId { get; set; }

    /// <summary>
    /// 值
    /// </summary>
    public int Value { get; set; }

    /// <summary>
    /// 百分比
    /// </summary>
    public double Percent { get; set; }

    /// <summary>
    /// 总体百分比
    /// </summary>
    public double PercentTotal { get; set; }
}