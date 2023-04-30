// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text.Encodings.Web;
using System.Text.Json.Serialization.Metadata;

namespace Snap.Hutao.Core.Json;

/// <summary>
/// Json 选项
/// </summary>
internal static class JsonOptions
{
    /// <summary>
    /// 默认的Json序列化选项
    /// </summary>
    public static readonly JsonSerializerOptions Default = new()
    {
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        TypeInfoResolver = new DefaultJsonTypeInfoResolver()
        {
            Modifiers =
            {
                JsonTypeInfoResolvers.ResolveEnumType,
            },
        },
    };
}