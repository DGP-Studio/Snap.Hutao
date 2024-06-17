// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using System.Globalization;

namespace Snap.Hutao.Web.Hutao.SpiralAbyss;

/// <summary>
/// 圣遗物套装
/// </summary>
[HighQuality]
internal sealed class ReliquarySet : IEquatable<ReliquarySet>
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

    public bool Equals(ReliquarySet? other)
    {
        if (other is null)
        {
            return false;
        }

        if (ReferenceEquals(this, other))
        {
            return true;
        }

        return EquipAffixId == other.EquipAffixId && Count == other.Count;
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ReliquarySet);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(EquipAffixId, Count);
    }

    public override string ToString()
    {
        return $"{EquipAffixId.Value}-{Count}";
    }
}
