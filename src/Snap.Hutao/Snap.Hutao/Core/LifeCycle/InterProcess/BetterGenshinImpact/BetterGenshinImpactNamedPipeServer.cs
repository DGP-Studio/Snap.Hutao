// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.LifeCycle.InterProcess.BetterGenshinImpact.Cultivation;
using Snap.Hutao.Core.LifeCycle.InterProcess.BetterGenshinImpact.Task;

namespace Snap.Hutao.Core.LifeCycle.InterProcess.BetterGenshinImpact;

[Service(ServiceLifetime.Singleton)]
internal sealed partial class BetterGenshinImpactNamedPipeServer
{
    private readonly IAutomationCultivationService automationCultivationService;
    private readonly ILogger<BetterGenshinImpactNamedPipeServer> logger;
    private readonly IAutomationTaskService automationTaskService;

    [GeneratedConstructor]
    public partial BetterGenshinImpactNamedPipeServer(IServiceProvider serviceProvider);

    public PipeResponse DispatchRequest(PipeRequest<JsonElement>? request)
    {
        if (request is not null)
        {
            switch (request.Kind)
            {
                case PipeRequestKind.Log:
                    logger.LogInformation("BGI: {log}", request.Data.GetString());
                    break;

                case PipeRequestKind.CreateOneShotTask:
                    {
                        if (request.Data.Deserialize<AutomationTaskDefinition>() is { } taskDefinition)
                        {
                            return automationTaskService.CreateOneShotTask(taskDefinition);
                        }
                    }

                    break;

                case PipeRequestKind.CreateSteppedTask:
                    {
                        if (request.Data.Deserialize<SteppedAutomationTaskDefinition>() is { } taskDefinition)
                        {
                            return automationTaskService.CreateSteppedTask(taskDefinition);
                        }
                    }

                    break;

                case PipeRequestKind.RemoveTask:
                    {
                        if (request.Data.Deserialize<string>() is { } id)
                        {
                            return automationTaskService.RemoveTask(id);
                        }
                    }

                    break;

                case PipeRequestKind.UpdateTaskDefinition:
                    {
                        if (request.Data.Deserialize<AutomationTaskDefinition>() is { } update)
                        {
                            return automationTaskService.UpdateTaskDefinition(update.Id, update.Name, update.Description);
                        }
                    }

                    break;

                case PipeRequestKind.UpdateTaskStepDefinition:
                    {
                        if (request.Data.Deserialize<UpdateAutomationTaskStepDefinition>() is { } update)
                        {
                            return automationTaskService.UpdateTaskStepDefinition(update.Id, update.Index, update.Description);
                        }
                    }

                    break;

                case PipeRequestKind.UpdateTaskStepIndex:
                    {
                        if (request.Data.Deserialize<UpdateAutomationTaskStepIndex>() is { } update)
                        {
                            return automationTaskService.UpdateTaskStepIndex(update.Id, update.Index);
                        }
                    }

                    break;

                case PipeRequestKind.AddTaskStepDefinition:
                    {
                        if (request.Data.Deserialize<AddAutomationTaskStepDefinition>() is { } add)
                        {
                            return automationTaskService.AddTaskStepDefinition(add.Id, add.Description);
                        }
                    }

                    break;

                case PipeRequestKind.QueryCurrentCultivationProject:
                    {
                        return new PipeResponse<AutomationCultivationProject>
                        {
                            Kind = PipeResponseKind.Object,
                            Data = automationCultivationService.GetCurrentProject(),
                        };
                    }

                case PipeRequestKind.BeginSwitchToNextGameAccount:
                    {
                        // TODO: Implement
                    }

                    break;
            }
        }

        return new PipeResponse<Void>
        {
            Kind = PipeResponseKind.None,
            Data = default,
        };
    }
}