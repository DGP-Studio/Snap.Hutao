// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.LifeCycle.InterProcess.BetterGenshinImpact.Task;

internal interface IAutomationTaskService
{
    PipeResponse CreateOneShotTask(AutomationTaskDefinition definition);

    PipeResponse CreateSteppedTask(SteppedAutomationTaskDefinition definition);

    PipeResponse RemoveTask(string id);

    PipeResponse UpdateTaskDefinition(string id, string name, string description);

    PipeResponse UpdateTaskStepDefinition(string id, int index, string description);

    PipeResponse UpdateTaskStepIndex(string id, int index);

    PipeResponse AddTaskStepDefinition(string id, string description);
}