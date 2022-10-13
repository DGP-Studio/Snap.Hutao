// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.Model;

/// <summary>
/// 排行包装
/// </summary>
public class RankInfo
{
    /// <summary>
    /// 造成伤害
    /// </summary>
    public ItemRate<int, double> Damage { get; set; } = default!;

    /// <summary>
    /// 受到伤害
    /// </summary>
    public ItemRate<int, double> TakeDamage { get; set; } = default!;
}
