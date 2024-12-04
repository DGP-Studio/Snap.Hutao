// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Bridge.Model;

internal sealed class CookieTokenPayload
{
    [JsonPropertyName("forceRefresh")]
    public bool ForceRefresh { get; set; }
}