// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Web.Hoyolab.Passport;

internal sealed class LTokenWrapper
{
    [JsonPropertyName("ltoken")]
    public string LToken { get; set; } = default!;
}