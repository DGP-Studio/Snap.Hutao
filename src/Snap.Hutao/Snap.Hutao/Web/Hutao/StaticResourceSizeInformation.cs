// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao;

internal sealed class StaticResourceSizeInformation
{
    [JsonPropertyName("original_full")]
    public long OriginalFull { get; init; }

    [JsonPropertyName("original_minimum")]
    public long OriginalMinimum { get; init; }

    [JsonPropertyName("tiny_full")]
    public long HighFull { get; init; }

    [JsonPropertyName("tiny_minimum")]
    public long HighMinimum { get; init; }
}