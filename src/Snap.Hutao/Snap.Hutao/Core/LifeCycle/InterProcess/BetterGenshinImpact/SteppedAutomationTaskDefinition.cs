// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.LifeCycle.InterProcess.BetterGenshinImpact;

internal sealed class SteppedAutomationTaskDefinition : AutomationTaskDefinition
{
    public required List<AutomationTaskStepDefinition> Steps { get; set; }

    public int CurrentStepIndex { get; set; }

    public bool IsIndeterminate { get; set; }
}