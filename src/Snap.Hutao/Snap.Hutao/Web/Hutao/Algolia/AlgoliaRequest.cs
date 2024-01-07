// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Hutao.Response;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Net.Http;

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