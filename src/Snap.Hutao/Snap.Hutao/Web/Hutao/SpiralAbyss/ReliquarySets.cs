// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hutao.SpiralAbyss.Converter;

namespace Snap.Hutao.Web.Hutao.SpiralAbyss;

/// <summary>
/// 包装圣遗物套装
/// </summary>
[HighQuality]
[JsonConverter(typeof(ReliquarySetsConverter))]
internal sealed class ReliquarySets : List<ReliquarySet>
{
    /// <summary>
    /// 构造一个新的圣遗物包装器
    /// </summary>
    /// <param name="sets">圣遗物套装</param>
    public ReliquarySets(IEnumerable<ReliquarySet> sets)
        : base(sets)
    {
    }
}
