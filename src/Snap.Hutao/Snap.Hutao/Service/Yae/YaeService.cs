// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.LifeCycle.InterProcess.Yae;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Model.InterChange.Achievement;
using Snap.Hutao.Model.InterChange.Inventory;
using Snap.Hutao.Service.Feature;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.Game.FileSystem;
using Snap.Hutao.Service.Game.Launching;
using Snap.Hutao.Service.Game.Launching.Context;
using Snap.Hutao.Service.Game.Launching.Invoker;
using Snap.Hutao.Service.Notification;
using Snap.Hutao.Service.User;
using Snap.Hutao.Service.Yae.Achievement;
using Snap.Hutao.Service.Yae.PlayerStore;
using Snap.Hutao.ViewModel.Game;
using Snap.Hutao.ViewModel.User;
using System.Diagnostics;

namespace Snap.Hutao.Service.Yae;

[GeneratedConstructor]
[Service(ServiceLifetime.Singleton, typeof(IYaeService))]
internal sealed partial class YaeService : IYaeService
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IServiceProvider serviceProvider;
    private readonly IFeatureService featureService;
    private readonly IUserService userService;
    private readonly ITaskContext taskContext;
    private readonly IMessenger messenger;

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
                    LaunchExecutionInvocationContext context = new()
                    {
                        ViewModel = viewModel,
                        ServiceProvider = serviceProvider,
                        LaunchOptions = serviceProvider.GetRequiredService<LaunchOptions>(),
                        Identity = GameIdentity.Create(userAndUid, viewModel.GameAccount),
                    };

                    if (!TryGetGameVersion(context, out string? version, out bool isOversea))
                    {
                        return default;
                    }

                    AchievementFieldId? fieldId = await featureService.GetAchievementFieldIdAsync(version).ConfigureAwait(false);
                    ArgumentNullException.ThrowIfNull(fieldId);

                    TargetNativeConfiguration config = TargetNativeConfiguration.Create(fieldId.NativeConfig, isOversea);
                    await new YaeLaunchExecutionInvoker(config, receiver).InvokeAsync(context).ConfigureAwait(false);

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
                catch (Exception ex)
                {
                    messenger.Send(InfoBarMessage.Error(ex));
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
                    LaunchExecutionInvocationContext context = new()
                    {
                        ViewModel = viewModel,
                        ServiceProvider = serviceProvider,
                        LaunchOptions = serviceProvider.GetRequiredService<LaunchOptions>(),
                        Identity = GameIdentity.Create(userAndUid, viewModel.GameAccount),
                    };

                    if (!TryGetGameVersion(context, out string? version, out bool isOversea))
                    {
                        return default;
                    }

                    AchievementFieldId? fieldId = await featureService.GetAchievementFieldIdAsync(version).ConfigureAwait(false);
                    ArgumentNullException.ThrowIfNull(fieldId);

                    TargetNativeConfiguration config = TargetNativeConfiguration.Create(fieldId.NativeConfig, isOversea);
                    await new YaeLaunchExecutionInvoker(config, receiver).InvokeAsync(context).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    messenger.Send(InfoBarMessage.Error(ex));
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

    private bool TryGetGameVersion(LaunchExecutionInvocationContext context, [NotNullWhen(true)] out string? version, out bool isOversea)
    {
        const string LockTrace = $"{nameof(YaeService)}.{nameof(TryGetGameVersion)}";

        if (context.LaunchOptions.TryGetGameFileSystem(LockTrace, out IGameFileSystem? gameFileSystem) is not GameFileSystemErrorKind.None)
        {
            context.ServiceProvider.GetRequiredService<IMessenger>().Send(InfoBarMessage.Error(SH.ServiceYaeGetGameVersionFailed));
        }

        if (gameFileSystem is null)
        {
            version = default;
            isOversea = false;
            return false;
        }

        using (gameFileSystem)
        {
            if (!gameFileSystem.TryGetGameVersion(out version) || string.IsNullOrEmpty(version))
            {
                messenger.Send(InfoBarMessage.Error(SH.ServiceYaeGetGameVersionFailed));
                isOversea = false;
                return false;
            }

            isOversea = gameFileSystem.IsExecutableOversea();
        }

        return true;
    }
}