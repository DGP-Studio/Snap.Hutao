// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.Algolia;

internal sealed class AlgoliaRequest
{
    [JsonPropertyName("query")]
    public string Query { get; set; } = default!;

    [JsonPropertyName("indexName")]
    public string IndexName { get; set; } = default!;

    [JsonPropertyName("params")]
    public string Params { get; set; } = default!;
}