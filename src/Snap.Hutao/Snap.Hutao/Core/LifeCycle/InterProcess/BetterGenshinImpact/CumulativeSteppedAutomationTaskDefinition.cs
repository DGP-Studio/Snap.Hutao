// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.LifeCycle.InterProcess.BetterGenshinImpact;

internal sealed class CumulativeSteppedAutomationTaskDefinition : AutomationTaskDefinition
{
    public required List<AutomationTaskStepDefinition> Steps { get; set; }
}