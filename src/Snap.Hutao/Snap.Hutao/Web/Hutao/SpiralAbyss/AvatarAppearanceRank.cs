// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;

namespace Snap.Hutao.Web.Hutao.SpiralAbyss;

/// <summary>
/// 出场率
/// </summary>
[HighQuality]
internal sealed class AvatarAppearanceRank
{
    /// <summary>
    /// 楼层
    /// </summary>
    public int Floor { get; set; }

    /// <summary>
    /// 排行
    /// </summary>
    public List<ItemRate<AvatarId, double>> Ranks { get; set; } = default!;
}