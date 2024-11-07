// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Core.LifeCycle.InterProcess.BetterGenshinImpact;

internal sealed class AutomationCultivaionEntry
{
    public required uint ItemId { get; set; }

    public required ImmutableArray<AutomationCultivaionItem> Items { get; set; }
}