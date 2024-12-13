// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using JetBrains.Annotations;
using System.Collections.Immutable;

namespace Snap.Hutao.Model.InterChange.GachaLog;

// ReSharper disable once InconsistentNaming
internal sealed class UIGFEntry<TItem>
{
    [JsonPropertyName("uid")]
    [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString | JsonNumberHandling.WriteAsString)]
    public required uint Uid { get; init; }

    [JsonPropertyName("timezone")]
    public required int TimeZone { get; init; }

    [JsonPropertyName("lang")]
    public string? Language { get; init; }

    [JsonPropertyName("list")]
    public required ImmutableArray<TItem> List { get; init; }

    [JsonIgnore]
    [UsedImplicitly]
    public bool IsSelected { get; set; }
}