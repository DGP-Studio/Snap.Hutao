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
[Injection(InjectAs.Scoped)]
internal sealed class ExperimentalFeaturesViewModel : ObservableObject
{
    private readonly IServiceProvider serviceProvider;

    /// <summary>
    /// 构造一个新的实验性功能视图模型
    /// </summary>
    /// <param name="serviceProvider">服务提供器</param>
    public ExperimentalFeaturesViewModel(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider;

        OpenCacheFolderCommand = new AsyncRelayCommand(OpenCacheFolderAsync);
        OpenDataFolderCommand = new AsyncRelayCommand(OpenDataFolderAsync);
        DeleteUsersCommand = new AsyncRelayCommand(DangerousDeleteUsersAsync);
    }

    /// <summary>
    /// 打开临时文件夹命令
    /// </summary>
    public ICommand OpenCacheFolderCommand { get; }

    /// <summary>
    /// 打开数据文件夹命令
    /// </summary>
    public ICommand OpenDataFolderCommand { get; }

    /// <summary>
    /// 清空用户命令
    /// </summary>
    public ICommand DeleteUsersCommand { get; }

    private Task OpenCacheFolderAsync()
    {
        HutaoOptions hutaoOptions = serviceProvider.GetRequiredService<HutaoOptions>();
        return Launcher.LaunchFolderPathAsync(hutaoOptions.LocalCache).AsTask();
    }

    private Task OpenDataFolderAsync()
    {
        HutaoOptions hutaoOptions = serviceProvider.GetRequiredService<HutaoOptions>();
        return Launcher.LaunchFolderPathAsync(hutaoOptions.DataFolder).AsTask();
    }

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