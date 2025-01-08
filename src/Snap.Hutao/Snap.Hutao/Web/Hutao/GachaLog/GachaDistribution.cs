// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Web.Hutao.GachaLog;

internal sealed class GachaDistribution
{
    public long TotalValidPulls { get; set; }

    public ImmutableArray<PullCount> Distribution { get; set; } = default!;
}