// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.LifeCycle.InterProcess.BetterGenshinImpact;

internal enum PipeRequestKind
{
    None,
    ContractVersion = 1,
    StartCapture = 2,                    // S to B
    StopCapture = 3,                     // S to B
    QueryTaskArray = 10,                 // S to B
    StartTask = 11,                      // S to B
    CreateOneShotTask = 20,              // B to S
    CreateSteppedTask = 21,              // B to S
    RemoveTask = 22,                     // B to S
    UpdateTaskDefinition = 23,           // B to S
    UpdateTaskStepDefinition = 24,       // B to S
    UpdateTaskStepIndex = 25,            // B to S
    AddTaskStepDefinition = 26,          // B to S
    BeginSwitchToNextGameAccount = 30,   // B to S
    EndSwitchToNextGameAccount = 31,     // S to B
    QueryCurrentCultivationProject = 40, // B to S
}