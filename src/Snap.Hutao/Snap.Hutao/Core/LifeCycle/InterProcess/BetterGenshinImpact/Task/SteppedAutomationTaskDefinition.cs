// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.LifeCycle.InterProcess.BetterGenshinImpact.Task;

internal sealed class SteppedAutomationTaskDefinition : AutomationTaskDefinition
{
    public required List<AutomationTaskStepDefinition> Steps { get; set; }

    public required int CurrentStepIndex { get; set; }

    public required bool IsIndeterminate { get; set; }
}