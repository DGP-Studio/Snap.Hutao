// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Service.AppCenter.Model.Log;

/// <summary>
/// 日志转换器
/// </summary>
public class LogConverter : JsonConverter<Log>
{
    /// <inheritdoc/>
    public override Log? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        throw Must.NeverHappen();
    }

    /// <inheritdoc/>
    public override void Write(Utf8JsonWriter writer, Log value, JsonSerializerOptions options)
    {
        JsonSerializer.Serialize(writer, value, value.GetType(), options);
    }
}