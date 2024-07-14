// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Core;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service.Game;
using Snap.Hutao.Service.User;
using Snap.Hutao.UI.Xaml.View.Dialog;

namespace Snap.Hutao.ViewModel.Setting;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class SettingDangerousFeatureViewModel : Abstraction.ViewModel
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly RuntimeOptions runtimeOptions;
    private readonly LaunchOptions launchOptions;
    private readonly IUserService userService;

    public RuntimeOptions RuntimeOptions { get => runtimeOptions; }

    public LaunchOptions LaunchOptions { get => launchOptions; }

    public bool IsAllocConsoleDebugModeEnabled
    {
        get => LocalSetting.Get(SettingKeys.IsAllocConsoleDebugModeEnabled, ConsoleWindowLifeTime.DebugModeEnabled);
        set
        {
            if (IsViewDisposed)
            {
                return;
            }

            ConfirmSetIsAllocConsoleDebugModeEnabledAsync(value).SafeForget();

            async ValueTask ConfirmSetIsAllocConsoleDebugModeEnabledAsync(bool value)
            {
                if (value)
                {
                    ReconfirmDialog dialog = await contentDialogFactory.CreateInstanceAsync<ReconfirmDialog>().ConfigureAwait(false);
                    if (await dialog.ConfirmAsync(SH.ViewSettingAllocConsoleHeader).ConfigureAwait(true))
                    {
                        LocalSetting.Set(SettingKeys.IsAllocConsoleDebugModeEnabled, true);
                        OnPropertyChanged(nameof(IsAllocConsoleDebugModeEnabled));
                        return;
                    }
                }

                LocalSetting.Set(SettingKeys.IsAllocConsoleDebugModeEnabled, false);
                OnPropertyChanged(nameof(IsAllocConsoleDebugModeEnabled));
            }
        }
    }

    public bool IsAdvancedLaunchOptionsEnabled
    {
        get => launchOptions.IsAdvancedLaunchOptionsEnabled;
        set
        {
            if (IsViewDisposed)
            {
                return;
            }

            ConfirmSetIsAdvancedLaunchOptionsEnabledAsync(value).SafeForget();

            async ValueTask ConfirmSetIsAdvancedLaunchOptionsEnabledAsync(bool value)
            {
                if (value)
                {
                    ReconfirmDialog dialog = await contentDialogFactory.CreateInstanceAsync<ReconfirmDialog>().ConfigureAwait(false);
                    if (await dialog.ConfirmAsync(SH.ViewPageSettingIsAdvancedLaunchOptionsEnabledHeader).ConfigureAwait(true))
                    {
                        launchOptions.IsAdvancedLaunchOptionsEnabled = true;
                        OnPropertyChanged(nameof(IsAdvancedLaunchOptionsEnabled));
                        return;
                    }
                }

                launchOptions.IsAdvancedLaunchOptionsEnabled = false;
                OnPropertyChanged(nameof(IsAdvancedLaunchOptionsEnabled));
            }
        }
    }

    [Command("DeleteUsersCommand")]
    private async Task DeleteUsersAsync()
    {
        if (userService is not IUserServiceUnsafe @unsafe)
        {
            return;
        }

        ContentDialogResult result = await contentDialogFactory
            .CreateForConfirmCancelAsync(SH.ViewDialogSettingDeleteUserDataTitle, SH.ViewDialogSettingDeleteUserDataContent)
            .ConfigureAwait(false);

        if (result is ContentDialogResult.Primary)
        {
            await @unsafe.UnsafeRemoveAllUsersAsync().ConfigureAwait(false);
            AppInstance.Restart(string.Empty);
        }
    }
}