// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.Primitive.Converter;

/// <summary>
/// 角色Id转换器
/// </summary>
internal class AvatarIdConverter : JsonConverter<AvatarId>
{
    /// <inheritdoc/>
    public override AvatarId Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return reader.GetInt32();
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, AvatarId value, JsonSerializerOptions options)
    {
        writer.WriteNumberValue(value);
    }
}