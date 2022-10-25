// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Primitive.Converter;

/// <summary>
/// 武器Id转换器
/// </summary>
internal class WeaponIdConverter : JsonConverter<WeaponId>
{
    /// <inheritdoc/>
    public override WeaponId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetInt32();
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, WeaponId value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}
