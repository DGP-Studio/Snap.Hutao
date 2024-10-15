// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao;

internal sealed class StaticResourceSizeInformation
{
    [JsonPropertyName("raw_full")]
    public long RawFull { get; set; }

    [JsonPropertyName("raw_minimum")]
    public long RawMinimum { get; set; }

    [JsonPropertyName("tiny_full")]
    public long HighFull { get; set; }

    [JsonPropertyName("tiny_minimum")]
    public long HighMinimum { get; set; }
}