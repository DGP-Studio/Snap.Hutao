// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.Model;

/// <summary>
/// 命座比例
/// </summary>
public class AvatarConstellation
{
    /// <summary>
    /// 角色ID
    /// </summary>
    public int Avatar { get; set; }

    /// <summary>
    /// 持有率
    /// </summary>
    public decimal HoldingRate { get; set; }

    /// <summary>
    /// 各命座比率
    /// </summary>
    public IEnumerable<Rate<int>> Rate { get; set; } = default!;
}