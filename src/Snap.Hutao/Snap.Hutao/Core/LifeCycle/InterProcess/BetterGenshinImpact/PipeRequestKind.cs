// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.LifeCycle.InterProcess.BetterGenshinImpact;

internal enum PipeRequestKind
{
    None,
    StartCapture = 1,              // S to B
    StopCapture = 2,               // S to B
    StartTask = 10,                // S to B
    CreateOneShotTask = 20,        // B to S
    CreateSteppedTask = 21,        // B to S
    RemoveTask = 22,               // B to S
    UpdateTaskDefinition = 23,     // B to S
    UpdateTaskStepDefinition = 24, // B to S
    UpdateTaskStepIndex = 25,      // B to S
    AddTaskStepDefinition = 26,    // B to S
    QueryGameAccountList = 30,     // B to S
    SwitchGameAccount = 31,        // B to S
    QueryCultivationList = 40,     // B to S
}