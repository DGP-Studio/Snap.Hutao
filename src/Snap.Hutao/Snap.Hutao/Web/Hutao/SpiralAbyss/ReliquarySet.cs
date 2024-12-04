// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Primitive;
using System.Globalization;

namespace Snap.Hutao.Web.Hutao.SpiralAbyss;

internal sealed class ReliquarySet : IEquatable<ReliquarySet>
{
    public ReliquarySet(string set)
        : this(set.AsSpan())
    {
    }

    public ReliquarySet(ReadOnlySpan<char> set)
    {
        if (set.TrySplitIntoTwo('-', out ReadOnlySpan<char> equipAffixId, out ReadOnlySpan<char> count))
        {
            EquipAffixId = uint.Parse(equipAffixId, CultureInfo.InvariantCulture);
            Count = int.Parse(count, CultureInfo.InvariantCulture);
        }
    }

    public ExtendedEquipAffixId EquipAffixId { get; }

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
