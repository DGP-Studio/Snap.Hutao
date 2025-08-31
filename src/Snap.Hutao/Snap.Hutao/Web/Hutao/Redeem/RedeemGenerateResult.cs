// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hutao.Redeem;

internal sealed class RedeemGenerateResult
{
    public ImmutableArray<string> Codes { get; set; }
}