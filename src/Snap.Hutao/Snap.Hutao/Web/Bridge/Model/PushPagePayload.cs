// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Bridge.Model;

internal sealed class PushPagePayload
{
    [JsonPropertyName("page")]
    public string Page { get; set; } = default!;
}