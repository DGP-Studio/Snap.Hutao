// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Response;

internal class ListWrapper<T>
{
    [JsonPropertyName("list")]
    public List<T> List { get; set; } = default!;
}