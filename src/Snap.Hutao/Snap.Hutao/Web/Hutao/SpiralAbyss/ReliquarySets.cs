// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hutao.SpiralAbyss.Converter;

namespace Snap.Hutao.Web.Hutao.SpiralAbyss;

[JsonConverter(typeof(ReliquarySetsConverter))]
internal sealed partial class ReliquarySets : List<ReliquarySet>, IEquatable<ReliquarySets>
{
    public ReliquarySets(IEnumerable<ReliquarySet> sets)
        : base(sets)
    {
    }

    public bool Equals(ReliquarySets? other)
    {
        if (other is null)
        {
            return false;
        }

        return ReferenceEquals(this, other) || this.SequenceEqual(other);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as ReliquarySets);
    }

    public override int GetHashCode()
    {
        HashCode hashCode = default;
        foreach (ReliquarySet set in this)
        {
            hashCode.Add(set);
        }

        return hashCode.ToHashCode();
    }
}
