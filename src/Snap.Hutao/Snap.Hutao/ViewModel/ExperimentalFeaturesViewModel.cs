// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Context.FileSystem.Location;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Snap.Hutao.Web.Hoyolab.Takumi.Event.BbsSignReward;
using Snap.Hutao.Web.Response;
using Windows.System;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 实验性功能视图模型
/// </summary>
[Injection(InjectAs.Transient)]
internal class ExperimentalFeaturesViewModel : ObservableObject
{
    private readonly IFileSystemLocation hutaoLocation;
    private readonly IUserService userService;
    private readonly SignClient signClient;
    private readonly IInfoBarService infoBarService;

    /// <summary>
    /// 构造一个新的实验性功能视图模型
    /// </summary>
    /// <param name="asyncRelayCommandFactory">异步命令工厂</param>
    /// <param name="hutaoLocation">数据文件夹</param>
    /// <param name="userService">用户服务</param>
    /// <param name="signClient">签到客户端</param>
    /// <param name="infoBarService">信息栏服务</param>
    public ExperimentalFeaturesViewModel(
        IAsyncRelayCommandFactory asyncRelayCommandFactory,
        HutaoLocation hutaoLocation,
        IUserService userService,
        SignClient signClient,
        IInfoBarService infoBarService)
    {
        this.hutaoLocation = hutaoLocation;
        this.userService = userService;
        this.signClient = signClient;
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

    private async Task SignAllUserGameRolesAsync()
    {
        foreach (Model.Binding.User user in await userService.GetUserCollectionAsync())
        {
            foreach (UserGameRole role in user.UserGameRoles)
            {
                Response<SignInResult>? result = await signClient.SignAsync(user, role);
                if (result != null)
                {
                    infoBarService.Information(result.Message);
                }

                await Task.Delay(TimeSpan.FromSeconds(15));
            }
        }
    }
}