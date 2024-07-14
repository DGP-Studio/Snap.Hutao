// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Model.InterChange.GachaLog;

internal sealed class UIGFEntry<TItem>
{
    [JsonPropertyName("uid")]
    public required string Uid { get; set; }

    [JsonPropertyName("timezone")]
    public required int TimeZone { get; set; }

    [JsonPropertyName("lang")]
    public string? Language { get; set; }

    [JsonPropertyName("list")]
    public required List<TItem> List { get; set; }

    [JsonIgnore]
    public bool IsSelected { get; set; }
}