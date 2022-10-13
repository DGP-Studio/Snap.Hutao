// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.Model;

/// <summary>
/// 出场率
/// </summary>
public class AvatarAppearanceRank
{
    /// <summary>
    /// 楼层
    /// </summary>
    public int Floor { get; set; }

    /// <summary>
    /// 排行
    /// </summary>
    public List<ItemRate<int, double>> Ranks { get; set; } = default!;
}