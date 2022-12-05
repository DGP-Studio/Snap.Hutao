// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Snap.Hutao.Context.Database;
using Snap.Hutao.Context.FileSystem.Location;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.User;
using Snap.Hutao.View.Dialog;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Hutao.Model.Post;
using Windows.Storage;
using Windows.System;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 实验性功能视图模型
/// </summary>
[Injection(InjectAs.Scoped)]
internal class ExperimentalFeaturesViewModel : ObservableObject
{
    private readonly IFileSystemLocation hutaoLocation;

    /// <summary>
    /// 构造一个新的实验性功能视图模型
    /// </summary>
    /// <param name="asyncRelayCommandFactory">异步命令工厂</param>
    /// <param name="hutaoLocation">数据文件夹</param>
    public ExperimentalFeaturesViewModel(
        IAsyncRelayCommandFactory asyncRelayCommandFactory,
        HutaoLocation hutaoLocation)
    {
        this.hutaoLocation = hutaoLocation;

        OpenCacheFolderCommand = asyncRelayCommandFactory.Create(OpenCacheFolderAsync);
        OpenDataFolderCommand = asyncRelayCommandFactory.Create(OpenDataFolderAsync);
        UploadSpiralAbyssRecordCommand = asyncRelayCommandFactory.Create(UploadSpiralAbyssRecordAsync);
        DeleteUsersCommand = asyncRelayCommandFactory.Create(DeleteUsersAsync);
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
    /// 上传深渊记录命令
    /// </summary>
    public ICommand UploadSpiralAbyssRecordCommand { get; }

    /// <summary>
    /// 清空用户命令
    /// </summary>
    public ICommand DeleteUsersCommand { get; }

    private Task OpenCacheFolderAsync()
    {
        return Launcher.LaunchFolderAsync(ApplicationData.Current.TemporaryFolder).AsTask();
    }

    private Task OpenDataFolderAsync()
    {
        return Launcher.LaunchFolderPathAsync(hutaoLocation.GetPath()).AsTask();
    }

    private async Task UploadSpiralAbyssRecordAsync()
    {
        HomaClient homaClient = Ioc.Default.GetRequiredService<HomaClient>();
        IUserService userService = Ioc.Default.GetRequiredService<IUserService>();
        IInfoBarService infoBarService = Ioc.Default.GetRequiredService<IInfoBarService>();

        if (userService.Current is Model.Binding.User.User user)
        {
            if (user.SelectedUserGameRole == null)
            {
                infoBarService.Warning("尚未选择角色");
            }

            SimpleRecord record = await homaClient.GetPlayerRecordAsync(user).ConfigureAwait(false);
            Web.Response.Response<string>? response = await homaClient.UploadRecordAsync(record).ConfigureAwait(false);

            if (response != null && response.IsOk())
            {
                infoBarService.Success(response.Message);
            }
        }
    }

    private async Task DeleteUsersAsync()
    {
        using (IServiceScope scope = Ioc.Default.CreateScope())
        {
            AppDbContext appDbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            await appDbContext.Users.ExecuteDeleteAsync().ConfigureAwait(false);

            IInfoBarService infoBarService = scope.ServiceProvider.GetRequiredService<IInfoBarService>();
            infoBarService.Success("清除用户数据成功,请重启胡桃");
        }
    }
}