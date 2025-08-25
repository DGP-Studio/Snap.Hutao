// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

namespace Snap.Hutao.Core.LifeCycle.InterProcess.BetterGenshinImpact.Task;

[Service(ServiceLifetime.Singleton, typeof(IAutomationTaskService))]
internal sealed class AutomationTaskService : IAutomationTaskService
{
    public PipeResponse CreateOneShotTask(AutomationTaskDefinition definition)
    {
        throw new NotImplementedException();
    }

    public PipeResponse CreateSteppedTask(SteppedAutomationTaskDefinition definition)
    {
        throw new NotImplementedException();
    }

    public PipeResponse RemoveTask(string id)
    {
        throw new NotImplementedException();
    }

    public PipeResponse UpdateTaskDefinition(string id, string name, string description)
    {
        throw new NotImplementedException();
    }

    public PipeResponse UpdateTaskStepDefinition(string id, int index, string description)
    {
        throw new NotImplementedException();
    }

    public PipeResponse UpdateTaskStepIndex(string id, int index)
    {
        throw new NotImplementedException();
    }

    public PipeResponse AddTaskStepDefinition(string id, string description)
    {
        throw new NotImplementedException();
    }
}