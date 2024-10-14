// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Text.Encodings.Web;
using System.Text.Json.Serialization.Metadata;

namespace Snap.Hutao.Core.Json;

internal static class JsonOptions
{
    public static readonly JsonSerializerOptions Default = new()
    {
        AllowTrailingCommas = true,
        ReadCommentHandling = JsonCommentHandling.Skip,
        DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
        NumberHandling = JsonNumberHandling.AllowReadingFromString,
        PropertyNameCaseInsensitive = true,
        WriteIndented = true,
        TypeInfoResolver = new DefaultJsonTypeInfoResolver
        {
            Modifiers =
            {
                JsonTypeInfoResolvers.ResolveEnumType,
            },
        },
    };
}