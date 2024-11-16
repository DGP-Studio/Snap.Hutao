// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Core.Logging;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Factory.ContentDialog;
using Snap.Hutao.Service.User;
using Snap.Hutao.UI.Xaml.View.Dialog;

namespace Snap.Hutao.ViewModel.Setting;

[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class SettingDangerousFeatureViewModel : Abstraction.ViewModel
{
    private readonly IContentDialogFactory contentDialogFactory;
    private readonly IUserService userService;

    public bool IsAllocConsoleDebugModeEnabled
    {
        get => LocalSetting.Get(SettingKeys.IsAllocConsoleDebugModeEnabled, ConsoleWindowLifeTime.DebugModeEnabled);
        set
        {
            if (IsViewDisposed)
            {
                return;
            }

            _ = ConfirmSetIsAllocConsoleDebugModeEnabledAsync(value);

            [SuppressMessage("", "SH003")]
            async Task ConfirmSetIsAllocConsoleDebugModeEnabledAsync(bool isEnabled)
            {
                if (isEnabled)
                {
                    ReconfirmDialog dialog = await contentDialogFactory.CreateInstanceAsync<ReconfirmDialog>().ConfigureAwait(false);
                    if (await dialog.ConfirmAsync(SH.ViewSettingAllocConsoleHeader).ConfigureAwait(true))
                    {
                        LocalSetting.Set(SettingKeys.IsAllocConsoleDebugModeEnabled, true);
                        OnPropertyChanged();
                        return;
                    }
                }

                LocalSetting.Set(SettingKeys.IsAllocConsoleDebugModeEnabled, false);
                OnPropertyChanged();
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