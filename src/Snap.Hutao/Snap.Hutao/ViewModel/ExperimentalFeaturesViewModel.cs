// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Context.FileSystem.Location;
using Snap.Hutao.Factory.Abstraction;
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
    public ExperimentalFeaturesViewModel(IAsyncRelayCommandFactory asyncRelayCommandFactory, HutaoLocation hutaoLocation)
    {
        this.hutaoLocation = hutaoLocation;

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

    private Task OpenCacheFolderAsync(CancellationToken token)
    {
        return Launcher.LaunchFolderAsync(App.CacheFolder).AsTask(token);
    }

    private Task OpenDataFolderAsync(CancellationToken token)
    {
        return Launcher.LaunchFolderPathAsync(hutaoLocation.GetPath()).AsTask(token);
    }
}