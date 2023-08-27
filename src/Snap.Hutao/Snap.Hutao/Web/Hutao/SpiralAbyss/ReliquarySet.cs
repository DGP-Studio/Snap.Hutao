// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using System.Globalization;

namespace Snap.Hutao.Web.Hutao.SpiralAbyss;

/// <summary>
/// 圣遗物套装
/// </summary>
[HighQuality]
internal sealed class ReliquarySet
{
    /// <summary>
    /// 构造一个新的圣遗物套装
    /// </summary>
    /// <param name="set">简单套装字符串</param>
    public ReliquarySet(string set)
    {
        string[]? deconstructed = set.Split('-');

        EquipAffixId = uint.Parse(deconstructed[0], CultureInfo.InvariantCulture);
        Count = int.Parse(deconstructed[1], CultureInfo.InvariantCulture);
    }

    /// <summary>
    /// Id
    /// </summary>
    public ExtendedEquipAffixId EquipAffixId { get; }

    /// <summary>
    /// 个数
    /// </summary>
    public int Count { get; }

    /// <inheritdoc/>
    public override string ToString()
    {
        return $"{EquipAffixId.Value}-{Count}";
    }
}
