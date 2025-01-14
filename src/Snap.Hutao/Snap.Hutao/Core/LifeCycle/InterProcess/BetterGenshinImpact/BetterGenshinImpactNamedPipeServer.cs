// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Service.Game;
using System.Collections.Immutable;

namespace Snap.Hutao.Core.LifeCycle.InterProcess.BetterGenshinImpact;

[ConstructorGenerated]
[Injection(InjectAs.Singleton)]
internal sealed partial class BetterGenshinImpactNamedPipeServer
{
    private readonly IAutomationTaskService automationTaskService;
    private readonly IGameService gameService;

    public PipeResponse DispatchRequest(PipeRequest<JsonElement>? request)
    {
        if (request is not null)
        {
            switch (request.Kind)
            {
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

                case PipeRequestKind.QueryGameAccountList:
                    {
                        if (request.Data.Deserialize<AutomationGameAccountType>() is { } type)
                        {
                            // ImmutableArray<AutomationGameAccount>.Builder builder = ImmutableArray.CreateBuilder<AutomationGameAccount>();
                            // if (type is AutomationGameAccountType.UseRegistry)
                            // {
                            //     try
                            //     {
                            //         ICollection<GameAccount> gameAccounts = gameService.GetGameAccountCollectionAsync().GetAwaiter().GetResult();
                            //         foreach (GameAccount account in gameAccounts)
                            //         {
                            //             builder.Add(new()
                            //             {
                            //                 Type = AutomationGameAccountType.UseRegistry | (account.Type is SchemeType.Oversea ? AutomationGameAccountType.Oversea : AutomationGameAccountType.Chinese),
                            //                 NameOrUserId = account.Name,
                            //             });
                            //         }
                            //     }
                            //     catch
                            //     {
                            //         builder.Clear();
                            //     }
                            // }
                        }
                    }

                    break;

                case PipeRequestKind.SwitchGameAccount:
                    {
                        // TODO: Implement
                    }

                    break;

                case PipeRequestKind.QueryCultivationList:
                    {
                        // TODO: Implement
                    }

                    break;
            }
        }

        return new()
        {
            Kind = PipeResponseKind.None,
            Data = default,
        };
    }
}