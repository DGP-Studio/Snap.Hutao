// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Context.FileSystem.Location;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.User;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Hutao.Model.Post;
using Windows.Storage;
using Windows.System;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 实验性功能视图模型
/// </summary>
[Injection(InjectAs.Transient)]
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

        if (userService.Current is Model.Binding.User user)
        {
            SimpleRecord record = await homaClient.GetPlayerRecordAsync(user).ConfigureAwait(false);
            Web.Response.Response<string>? response = await homaClient.UploadRecordAsync(record).ConfigureAwait(false);

            if (response != null && response.IsOk())
            {
                IInfoBarService infoBarService = Ioc.Default.GetRequiredService<IInfoBarService>();
                infoBarService.Success(response.Message);
            }
        }
    }
}