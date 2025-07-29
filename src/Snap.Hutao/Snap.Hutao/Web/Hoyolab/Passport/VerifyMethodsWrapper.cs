// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hoyolab.Passport;

internal sealed class VerifyMethodsWrapper
{
    [JsonPropertyName("verify_methods")]
    public required ImmutableArray<int> VerifyMethods { get; set; }
}