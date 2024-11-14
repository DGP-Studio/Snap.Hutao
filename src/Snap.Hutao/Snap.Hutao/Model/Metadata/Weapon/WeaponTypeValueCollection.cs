// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata.Weapon;

[JsonConverter(typeof(WeaponTypeValueCollectionConverter))]
internal sealed class WeaponTypeValueCollection
{
    private readonly SortedDictionary<FightProperty, GrowCurveType> typeValues = [];
    private readonly SortedDictionary<FightProperty, float> typeInitValues = [];

    public WeaponTypeValueCollection(ImmutableArray<WeaponTypeValue> entries)
    {
        foreach (ref readonly WeaponTypeValue entry in entries.AsSpan())
        {
            typeValues.Add(entry.Type, entry.Value);
            typeInitValues.Add(entry.Type, entry.InitValue);
        }
    }

    internal IReadOnlyDictionary<FightProperty, GrowCurveType> TypeValues { get => typeValues; }
}