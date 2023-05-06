// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Core;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model.Entity.Database;
using Windows.Storage;
using Windows.System;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 实验性功能视图模型
/// </summary>
[HighQuality]
[ConstructorGenerated]
[Injection(InjectAs.Scoped)]
internal sealed partial class ExperimentalFeaturesViewModel : ObservableObject
{
    private readonly IServiceProvider serviceProvider;

    [Command("OpenCacheFolderCommand")]
    private Task OpenCacheFolderAsync()
    {
        HutaoOptions hutaoOptions = serviceProvider.GetRequiredService<HutaoOptions>();
        return Launcher.LaunchFolderPathAsync(hutaoOptions.LocalCache).AsTask();
    }

    [Command("OpenDataFolderCommand")]
    private Task OpenDataFolderAsync()
    {
        HutaoOptions hutaoOptions = serviceProvider.GetRequiredService<HutaoOptions>();
        return Launcher.LaunchFolderPathAsync(hutaoOptions.DataFolder).AsTask();
    }

    [Command("DeleteUsersCommand")]
    private async Task DangerousDeleteUsersAsync()
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            ContentDialogResult result = await scope.ServiceProvider
                .GetRequiredService<IContentDialogFactory>()
                .ConfirmCancelAsync(SH.ViewDialogSettingDeleteUserDataTitle, SH.ViewDialogSettingDeleteUserDataContent)
                .ConfigureAwait(false);

            if (result == ContentDialogResult.Primary)
            {
                AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                await appDbContext.Users.ExecuteDeleteAsync().ConfigureAwait(false);

                AppInstance.Restart(string.Empty);
            }
        }
    }
}