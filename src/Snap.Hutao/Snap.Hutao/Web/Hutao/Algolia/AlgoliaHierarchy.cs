// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Hutao.Response;
using Snap.Hutao.Web.Request.Builder;
using Snap.Hutao.Web.Request.Builder.Abstraction;
using System.Net.Http;

namespace Snap.Hutao.Web.Hutao.Algolia;

internal sealed class AlgoliaHierarchy
{
    [JsonPropertyName("lvl0")]
    public string? Lvl0 { get; set; }

    [JsonPropertyName("lvl1")]
    public string? Lvl1 { get; set; }

    [JsonPropertyName("lvl2")]
    public string? Lvl2 { get; set; }

    [JsonPropertyName("lvl3")]
    public string? Lvl3 { get; set; }

    [JsonPropertyName("lvl4")]
    public string? Lvl4 { get; set; }

    [JsonPropertyName("lvl5")]
    public string? Lvl5 { get; set; }

    [JsonPropertyName("lvl6")]
    public string? Lvl6 { get; set; }
}