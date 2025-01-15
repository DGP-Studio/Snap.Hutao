// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.
using System.Collections.Immutable;

namespace Snap.Hutao.Core.LifeCycle.InterProcess.BetterGenshinImpact.Cultivation;

internal sealed class AutomationCultivationProject
{
    public required ImmutableArray<AutomationCultivationEntry> Entries { get; set; }

    public required ImmutableArray<AutomationInventoryItem> InventoryItems { get; set; }
}