// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Convert;

namespace Snap.Hutao.Model.Primitive.Converter;

/// <summary>
/// Id 转换器
/// </summary>
/// <typeparam name="TWrapper">包装类型</typeparam>
internal class IdentityConverter<TWrapper> : JsonConverter<TWrapper>
    where TWrapper : struct
{
    /// <inheritdoc/>
    public override TWrapper Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        return CastTo<TWrapper>.From(reader.GetInt32());
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, TWrapper value, JsonSerializerOptions options)
    {

        writer.WriteNumberValue(CastTo<int>.From(value));
    }
}