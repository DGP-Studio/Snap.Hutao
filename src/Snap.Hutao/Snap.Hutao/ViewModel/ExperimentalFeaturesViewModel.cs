// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.UI.Xaml.Controls;
using Microsoft.Windows.AppLifecycle;
using Snap.Hutao.Core;
using Snap.Hutao.Core.LifeCycle;
using Snap.Hutao.Core.Setting;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model.Entity.Database;
using Snap.Hutao.Service.Notification;
using Windows.ApplicationModel.Core;
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

    [Command("SwitchToStarRailToolCommand")]
    private async Task SwitchToStarRailToolAsync()
    {
        if (!StaticResource.IsSwitchToStarRailTool())
        {
            using (IServiceScope scope = serviceProvider.CreateScope())
            {
                ContentDialogResult result = await scope.ServiceProvider
                    .GetRequiredService<IContentDialogFactory>()
                    .ConfirmCancelAsync(SH.ViewDialogSettingSwitchToStarRailTitle, SH.ViewDialogSettingSwitchToStarRailToolsContent)
                    .ConfigureAwait(false);

                if (result != ContentDialogResult.Primary)
                {
                    return;
                }
            }
        }

        StaticResource.SwitchBetweenStarRailOrGenshin();
        AppInstance.Restart(string.Empty);
    }

    [Command("IncludeInSelfStartCommand")]
    private async Task IncludeInSelfStartAsync()
    {
        using (IServiceScope scope = serviceProvider.CreateScope())
        {
            ContentDialogResult result = await scope.ServiceProvider
                .GetRequiredService<IContentDialogFactory>()
                .ConfirmCancelAsync(IncludeOrExcludeSelfStartInfo(0), IncludeOrExcludeSelfStartInfo(1))
                .ConfigureAwait(false);

            if (result != ContentDialogResult.Primary)
            {
                return;
            }
        }

        CoreApplication.EnablePrelaunch(!Activation.IsIncludedInSelfStart(true));

        IInfoBarService infoBarService = serviceProvider.GetRequiredService<IInfoBarService>();
        infoBarService.Success(IncludeOrExcludeSelfStartInfo(0, true));
    }

    /// <summary>
    /// 用于切换来自 ExperimentalFeaturesViewModel.IncludeInSelfStartAsync() 中 ContentDialog 中的 Title 和 Content 信息
    /// 为了化简代码复杂程度
    /// </summary>
    /// <param name="index">下标 用于确定 Title 或 Content</param>
    /// <returns>对应的内容</returns>
    private string IncludeOrExcludeSelfStartInfo(int index, bool successfulContent = false)
    {
        if (index > 1)
        {
            return string.Empty;
        }

        if (!successfulContent)
        {
            // init default content for the ContentDialog
            string[] includeContent = { SH.ViewDialogSettingIncludeSelfStartTitle, SH.ViewDialogSettingIncludeSelfStartContent };
            string[] excludeContent = { SH.ViewDialogSettingExcludeSelfStartTitle, SH.ViewDialogSettingExcludeSelfStartContent };

            bool isIncludedInSelfStart = Activation.IsIncludedInSelfStart();

            return isIncludedInSelfStart == false ? includeContent[index] : excludeContent[index];
        }
        else
        {
            bool isIncludedInSelfStart = Activation.IsIncludedInSelfStart();

            return isIncludedInSelfStart == true ? SH.ViewDialogSettingIncludeSelfStartSuccessfulContent : SH.ViewDialogSettingExcludeSelfStartSuccessfulContent;
        }
    }
}