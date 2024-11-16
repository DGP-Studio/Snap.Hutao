// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Model.InterChange.GachaLog;

internal sealed class UIGFEntry<TItem>
{
    [JsonPropertyName("uid")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public required uint Uid { get; set; }

    [JsonPropertyName("timezone")]
    public required int TimeZone { get; set; }

    [JsonPropertyName("lang")]
    public string? Language { get; set; }

    [JsonPropertyName("list")]
    public required ImmutableArray<TItem> List { get; set; }

    [JsonIgnore]
    public bool IsSelected { get; set; }
}