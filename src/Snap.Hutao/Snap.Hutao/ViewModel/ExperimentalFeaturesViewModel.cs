// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Factory.Abstraction;
using Windows.Storage;
using Windows.System;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 实验性功能视图模型
/// </summary>
[Injection(InjectAs.Transient)]
internal class ExperimentalFeaturesViewModel : ObservableObject
{
    /// <summary>
    /// 构造一个新的实验性功能视图模型
    /// </summary>
    /// <param name="asyncRelayCommandFactory">异步命令工厂</param>
    public ExperimentalFeaturesViewModel(IAsyncRelayCommandFactory asyncRelayCommandFactory)
    {
        OpenCacheFolderCommand = asyncRelayCommandFactory.Create(OpenCacheFolderAsync);
        OpenDataFolderCommand = asyncRelayCommandFactory.Create(OpenDataFolderAsync);
    }

    /// <summary>
    /// 打开临时文件夹命令
    /// </summary>
    public ICommand OpenCacheFolderCommand { get; }

    /// <summary>
    /// 打开数据文件夹命令
    /// </summary>
    public ICommand OpenDataFolderCommand { get; }

    private Task OpenCacheFolderAsync()
    {
        return Launcher.LaunchFolderAsync(App.AppData.TemporaryFolder).AsTask();
    }

    private async Task OpenDataFolderAsync()
    {
        StorageFolder folder = await KnownFolders.DocumentsLibrary.GetFolderAsync("Hutao").AsTask().ConfigureAwait(false);
        await Launcher.LaunchFolderAsync(folder).AsTask().ConfigureAwait(false);
    }
}