// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.Collections.Immutable;

namespace Snap.Hutao.Core.LifeCycle.InterProcess.BetterGenshinImpact;

internal sealed class FixedSteppedAutomationTaskDefinition : AutomationTaskDefinition
{
    public required ImmutableArray<AutomationTaskStepDefinition> Steps { get; set; }

    public int CurrentStepIndex { get; set; }
}