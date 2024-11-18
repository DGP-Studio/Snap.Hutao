// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.LifeCycle.InterProcess.BetterGenshinImpact;

internal enum PipeRequestKind
{
    None,
    StartCapture = 1,
    StopCapture = 2,

    CreateOneshotTask = 10,
    CreateFixedSteppedTask = 11,
    CreateCumulativeSteppedTask = 12,
    RemoveTask = 13,
    UpdateTaskDefinition = 14,
    IncreaseTaskStepIndex = 15,
    AddTaskStep = 16,

    QueryGameAccountList = 30,
    SwitchGameAccount = 31,

    QueryCultivaionList = 40,
}