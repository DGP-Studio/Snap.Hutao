// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hutao.Algolia;

internal sealed class AlgoliaResult
{
    [JsonPropertyName("hits")]
    public List<AlgoliaHit> Hits { get; set; } = default!;
}