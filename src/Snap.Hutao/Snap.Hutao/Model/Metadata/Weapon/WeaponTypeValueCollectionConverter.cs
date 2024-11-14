// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Model.Metadata.Weapon;

internal sealed class WeaponTypeValueCollectionConverter : JsonConverter<WeaponTypeValueCollection>
{
    public override WeaponTypeValueCollection? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return new(JsonSerializer.Deserialize<ImmutableArray<WeaponTypeValue>>(ref reader, options));
    }

    public override void Write(Utf8JsonWriter writer, WeaponTypeValueCollection value, JsonSerializerOptions options)
    {
        // TODO: impl correctly
        JsonSerializer.Serialize(writer, value.TypeValues, options);
    }
}