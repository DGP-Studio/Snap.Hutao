// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.LifeCycle.InterProcess.BetterGenshinImpact;

internal sealed class UpdateAutomationTaskStepDefinition
{
    public required string Id { get; set; }

    public required int Index { get; set; }

    public required string Description { get; set; }
}

internal sealed class UpdateAutomationTaskStepIndex
{
    public required string Id { get; set; }

    public required int Index { get; set; }
}