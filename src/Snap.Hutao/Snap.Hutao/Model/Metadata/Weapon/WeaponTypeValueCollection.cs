// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Intrinsic;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata.Weapon;

[JsonConverter(typeof(Converter))]
internal sealed class WeaponTypeValueCollection
{
    private readonly SortedDictionary<FightProperty, GrowCurveType> typeValues = [];
    private readonly SortedDictionary<FightProperty, float> typeInitValues = [];

    public WeaponTypeValueCollection(ImmutableArray<WeaponTypeValue> array)
    {
        Array = array;
        foreach (ref readonly WeaponTypeValue entry in array.AsSpan())
        {
            typeValues.Add(entry.Type, entry.Value);
            typeInitValues.Add(entry.Type, entry.InitValue);
        }
    }

    internal ImmutableArray<WeaponTypeValue> Array { get; }

    internal IReadOnlyDictionary<FightProperty, GrowCurveType> TypeValues { get => typeValues; }

    internal IReadOnlyDictionary<FightProperty, float> TypeInitValues { get => typeInitValues; }
}

[SuppressMessage("", "SA1402")]
file sealed class Converter : JsonConverter<WeaponTypeValueCollection>
{
    public override WeaponTypeValueCollection Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new(JsonSerializer.Deserialize<ImmutableArray<WeaponTypeValue>>(ref reader, options));
    }

    public override void Write(Utf8JsonWriter writer, WeaponTypeValueCollection value, JsonSerializerOptions options)
    {
        throw new JsonException();
    }
}