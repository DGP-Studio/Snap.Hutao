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
[Service(ServiceLifetime.Singleton, typeof(IYaeService))]
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
            using (YaeDataArrayReceiver receiver = new())
            {
                try
                {
                    UserAndUid? userAndUid = await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false);
                    using (LaunchExecutionContext context = new(serviceProvider, viewModel, userAndUid))
                    {
                        if (!context.TryGetGameFileSystem(out IGameFileSystemView? gameFileSystem) ||
                            !gameFileSystem.TryGetGameVersion(out string? version) ||
                            string.IsNullOrEmpty(version))
                        {
                            infoBarService.Error(SH.ServiceYaeGetGameVersionFailed);
                            return default;
                        }

                        AchievementFieldId? fieldId = await featureService.GetAchievementFieldIdAsync(version).ConfigureAwait(false);
                        ArgumentNullException.ThrowIfNull(fieldId);

                        TargetNativeConfiguration config = TargetNativeConfiguration.Create(fieldId.NativeConfig, context.TargetScheme.IsOversea);
                        LaunchExecutionResult result = await new YaeLaunchExecutionInvoker(config, receiver).InvokeAsync(context).ConfigureAwait(false);

                        if (result.Kind is not LaunchExecutionResultKind.Ok)
                        {
                            infoBarService.Warning(result.ErrorMessage);
                            return default;
                        }

                        UIAF? uiaf = default;
                        foreach (YaeData data in receiver.Array)
                        {
                            using (data)
                            {
                                if (data.Kind is YaeCommandKind.ResponseAchievement)
                                {
                                    Debug.Assert(uiaf is null);
                                    uiaf = AchievementParser.Parse(data.Bytes, fieldId);
                                }
                            }
                        }

                        return uiaf;
                    }
                }
                catch (Exception ex)
                {
                    infoBarService.Error(ex);
                    return default;
                }
            }
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
            UIIF? uiif = default;
            Dictionary<InterestedPropType, double> propMap = [];
            using (YaeDataArrayReceiver receiver = new())
            {
                try
                {
                    UserAndUid? userAndUid = await userService.GetCurrentUserAndUidAsync().ConfigureAwait(false);
                    using (LaunchExecutionContext context = new(serviceProvider, viewModel, userAndUid))
                    {
                        if (!context.TryGetGameFileSystem(out IGameFileSystemView? gameFileSystem) ||
                            !gameFileSystem.TryGetGameVersion(out string? version) ||
                            string.IsNullOrEmpty(version))
                        {
                            infoBarService.Error(SH.ServiceYaeGetGameVersionFailed);
                            return default;
                        }

                        AchievementFieldId? fieldId = await featureService.GetAchievementFieldIdAsync(version).ConfigureAwait(false);
                        ArgumentNullException.ThrowIfNull(fieldId);

                        TargetNativeConfiguration config = TargetNativeConfiguration.Create(fieldId.NativeConfig, context.TargetScheme.IsOversea);
                        LaunchExecutionResult result = await new YaeLaunchExecutionInvoker(config, receiver).InvokeAsync(context).ConfigureAwait(false);

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

                foreach (YaeData data in receiver.Array)
                {
                    using (data)
                    {
                        switch (data.Kind)
                        {
                            case YaeCommandKind.ResponsePlayerStore:
                                Debug.Assert(uiif is null);
                                uiif = PlayerStoreParser.Parse(data.Bytes);
                                break;
                            case YaeCommandKind.ResponsePlayerProp:
                                {
                                    ref readonly YaePropertyTypeValue typeValue = ref data.PropertyTypeValue;
                                    propMap.Add(typeValue.Type, typeValue.Value);
                                    break;
                                }
                        }
                    }
                }
            }

            if (uiif is null)
            {
                return default;
            }

            // Unfortunately, we store data in uint rather than double, so we have to truncate the value.
            double count = propMap.GetValueOrDefault(InterestedPropType.PlayerSCoin) - propMap.GetValueOrDefault(InterestedPropType.PlayerWaitSubSCoin);
            UIIFItem mora = UIIFItem.From(202U, (uint)Math.Clamp(count, uint.MinValue, uint.MaxValue));

            return uiif.WithList([mora, .. uiif.List]);
        }
    }
}