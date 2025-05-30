// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.LifeCycle.InterProcess.Yae;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model.InterChange.Achievement;
using Snap.Hutao.Model.InterChange.Inventory;
using Snap.Hutao.Service.Feature;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.Launching;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.Service.Yae.Achievement;
using Snap.Hutao.Service.Yae.PlayerStore;
using Snap.Hutao.ViewModel.Game;
using Snap.Hutao.ViewModel.User;
using System.Diagnostics;

namespace Snap.Hutao.Service.Yae;

[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IYaeService))]
internal sealed partial class YaeService : IYaeService
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly IFeatureService featureService;
    private readonly IInfoBarService infoBarService;
    private readonly IUserService userService;
    private readonly ITaskContext taskContext;

    public async ValueTask<UIAF?> GetAchievementAsync(IViewModelSupportLaunchExecution viewModel)
    {
        ContentDialog dialog = await contentDialogFactory
            .CreateForIndeterminateProgressAsync(SH.ServiceYaeWaitForGameResponseMessage)
            .ConfigureAwait(false);

        using (await contentDialogFactory.BlockAsync(dialog).ConfigureAwait(false))
        {
            await taskContext.SwitchToBackgroundAsync();
            YaeDataArrayReceiver receiver = new();
            string? version = default;
            try
            {
                UserAndUid? userAndUid = await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false);
                using (LaunchExecutionContext context = new(serviceProvider, viewModel, userAndUid))
                {
                    if (context.TryGetGameFileSystem(out IGameFileSystemView? gameFileSystem))
                    {
                        _ = gameFileSystem.TryGetGameVersion(out version);
                    }

                    LaunchExecutionResult result = await new YaeLaunchExecutionInvoker(receiver).InvokeAsync(context).ConfigureAwait(false);

                    if (result.Kind is not LaunchExecutionResultKind.Ok)
                    {
                        infoBarService.Warning(result.ErrorMessage);
                        return default;
                    }
                }
            }
            catch (Exception ex)
            {
                infoBarService.Error(ex);
                return default;
            }

            UIAF? uiaf = default;
            foreach (YaeData data in receiver.Array)
            {
                using (data)
                {
                    if (data.Kind is YaeDataKind.Achievement)
                    {
                        Debug.Assert(uiaf is null);
                        AchievementFieldId? fieldId = default;
                        if (!string.IsNullOrEmpty(version))
                        {
                            fieldId = await featureService.GetAchievementFieldIdFeatureAsync(version);
                        }

                        uiaf = AchievementParser.Parse(data.Bytes, fieldId);
                    }
                }
            }

            return uiaf;
        }
    }

    public async ValueTask<UIIF?> GetInventoryAsync(IViewModelSupportLaunchExecution viewModel)
    {
        ContentDialog dialog = await contentDialogFactory
            .CreateForIndeterminateProgressAsync(SH.ServiceYaeWaitForGameResponseMessage)
            .ConfigureAwait(false);

        using (await contentDialogFactory.BlockAsync(dialog).ConfigureAwait(false))
        {
            await taskContext.SwitchToBackgroundAsync();
            YaeDataArrayReceiver receiver = new();

            try
            {
                UserAndUid? userAndUid = await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false);
                using (LaunchExecutionContext context = new(serviceProvider, viewModel, userAndUid))
                {
                    LaunchExecutionResult result = await new YaeLaunchExecutionInvoker(receiver).InvokeAsync(context).ConfigureAwait(false);

                    if (result.Kind is not LaunchExecutionResultKind.Ok)
                    {
                        infoBarService.Warning(result.ErrorMessage);
                        return default;
                    }
                }
            }
            catch (Exception ex)
            {
                infoBarService.Error(ex);
                return default;
            }

            UIIF? uiif = default;
            Dictionary<InterestedPropType, double> propMap = [];
            foreach (YaeData data in receiver.Array)
            {
                using (data)
                {
                    if (data.Kind is YaeDataKind.PlayerStore)
                    {
                        Debug.Assert(uiif is null);
                        uiif = PlayerStoreParser.Parse(data.Bytes);
                    }
                    else if (data.Kind is YaeDataKind.VirtualItem)
                    {
                        ref readonly YaePropertyTypeValue typeValue = ref data.PropertyTypeValue;
                        propMap.Add(typeValue.Type, typeValue.Value);
                    }
                }
            }

            if (uiif is null)
            {
                return default;
            }

            double count = propMap.GetValueOrDefault(InterestedPropType.PlayerSCoin) - propMap.GetValueOrDefault(InterestedPropType.PlayerWaitSubSCoin);
            UIIFItem mora = UIIFItem.From(202U, (uint)Math.Clamp(count, uint.MinValue, uint.MaxValue));

            return new()
            {
                Info = uiif.Info,
                List = [mora, .. uiif.List],
            };
        }
    }
}