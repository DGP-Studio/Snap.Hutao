// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Context.FileSystem.Location;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Sign;
using System.Text;
using Windows.System;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 实验性功能视图模型
/// </summary>
[Injection(InjectAs.Transient)]
internal class ExperimentalFeaturesViewModel : ObservableObject
{
    private readonly IFileSystemLocation hutaoLocation;
    private readonly ISignService signService;
    private readonly IInfoBarService infoBarService;

    /// <summary>
    /// 构造一个新的实验性功能视图模型
    /// </summary>
    /// <param name="asyncRelayCommandFactory">异步命令工厂</param>
    /// <param name="hutaoLocation">数据文件夹</param>
    /// <param name="signService">签到客户端</param>
    /// <param name="infoBarService">信息条服务</param>
    public ExperimentalFeaturesViewModel(
        IAsyncRelayCommandFactory asyncRelayCommandFactory,
        HutaoLocation hutaoLocation,
        ISignService signService,
        IInfoBarService infoBarService)
    {
        this.hutaoLocation = hutaoLocation;
        this.signService = signService;
        this.infoBarService = infoBarService;

        OpenCacheFolderCommand = asyncRelayCommandFactory.Create(OpenCacheFolderAsync);
        OpenDataFolderCommand = asyncRelayCommandFactory.Create(OpenDataFolderAsync);
        SignAllUserGameRolesCommand = asyncRelayCommandFactory.Create(SignAllUserGameRolesAsync);
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
    /// 签到全部角色命令
    /// </summary>
    public ICommand SignAllUserGameRolesCommand { get; }

    private Task OpenCacheFolderAsync()
    {
        return Launcher.LaunchFolderAsync(App.CacheFolder).AsTask();
    }

    private Task OpenDataFolderAsync()
    {
        return Launcher.LaunchFolderPathAsync(hutaoLocation.GetPath()).AsTask();
    }

    private async Task SignAllUserGameRolesAsync(CancellationToken token)
    {
        SignResult result = await signService.SignForAllAsync(token);

        StringBuilder stringBuilder = new StringBuilder()
            .Append($"签到完成 - 用时: {result.Time.TotalSeconds:F2} 秒\r\n")
            .Append($"请求: {result.TotalCount} 次\r\n")
            .Append($"补签: {result.RetryCount} 次");

        infoBarService.Information(stringBuilder.ToString());
    }
}