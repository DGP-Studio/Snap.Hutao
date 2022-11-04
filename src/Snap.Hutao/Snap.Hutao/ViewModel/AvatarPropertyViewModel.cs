// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Snap.Hutao.Control;
using Snap.Hutao.Core.Threading;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model.Binding.AvatarProperty;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.AvatarInfo;
using Snap.Hutao.Service.User;
using Snap.Hutao.View.Dialog;
using Snap.Hutao.Web.Hoyolab;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 角色属性视图模型
/// </summary>
[Injection(InjectAs.Scoped)]
internal class AvatarPropertyViewModel : ObservableObject, ISupportCancellation
{
    private readonly IUserService userService;
    private readonly IAvatarInfoService avatarInfoService;
    private readonly IInfoBarService infoBarService;
    private Summary? summary;
    private Avatar? selectedAvatar;

    /// <summary>
    /// 构造一个新的角色属性视图模型
    /// </summary>
    /// <param name="userService">用户服务</param>
    /// <param name="avatarInfoService">角色信息服务</param>
    /// <param name="asyncRelayCommandFactory">异步命令工厂</param>
    /// <param name="infoBarService">信息条服务</param>
    public AvatarPropertyViewModel(
        IUserService userService,
        IAvatarInfoService avatarInfoService,
        IAsyncRelayCommandFactory asyncRelayCommandFactory,
        IInfoBarService infoBarService)
    {
        this.userService = userService;
        this.avatarInfoService = avatarInfoService;
        this.infoBarService = infoBarService;

        OpenUICommand = asyncRelayCommandFactory.Create(OpenUIAsync);
        RefreshByUserGameRoleCommand = asyncRelayCommandFactory.Create(RefreshByUserGameRoleAsync);
        RefreshByInputUidCommand = asyncRelayCommandFactory.Create(RefreshByInputUidAsync);
    }

    /// <inheritdoc/>
    public CancellationToken CancellationToken { get; set; }

    /// <summary>
    /// 简述对象
    /// </summary>
    public Summary? Summary { get => summary; set => SetProperty(ref summary, value); }

    /// <summary>
    /// 选中的角色
    /// </summary>
    public Avatar? SelectedAvatar { get => selectedAvatar; set => SetProperty(ref selectedAvatar, value); }

    /// <summary>
    /// 加载界面命令
    /// </summary>
    public ICommand OpenUICommand { get; set; }

    /// <summary>
    /// 按当前角色刷新命令
    /// </summary>
    public ICommand RefreshByUserGameRoleCommand { get; set; }

    /// <summary>
    /// 按UID刷新命令
    /// </summary>
    public ICommand RefreshByInputUidCommand { get; set; }

    private Task OpenUIAsync()
    {
        if (userService.Current is Model.Binding.User user)
        {
            if (user.SelectedUserGameRole is UserGameRole role)
            {
                return RefreshCoreAsync((PlayerUid)role, RefreshOption.DatabaseOnly, CancellationToken);
            }
        }

        return Task.CompletedTask;
    }

    private Task RefreshByUserGameRoleAsync()
    {
        if (userService.Current is Model.Binding.User user)
        {
            if (user.SelectedUserGameRole is UserGameRole role)
            {
                return RefreshCoreAsync((PlayerUid)role, RefreshOption.Standard, CancellationToken);
            }
        }

        return Task.CompletedTask;
    }

    private async Task RefreshByInputUidAsync()
    {
        MainWindow mainWindow = Ioc.Default.GetRequiredService<MainWindow>();
        (bool isOk, PlayerUid uid) = await new AvatarInfoQueryDialog(mainWindow).GetPlayerUidAsync().ConfigureAwait(false);

        if (isOk)
        {
            await RefreshCoreAsync(uid, RefreshOption.RequestFromAPI, CancellationToken).ConfigureAwait(false);
        }
    }

    private async Task RefreshCoreAsync(PlayerUid uid, RefreshOption option, CancellationToken token)
    {
        try
        {
            (RefreshResult result, Summary? summary) = await avatarInfoService.GetSummaryAsync(uid, option, token).ConfigureAwait(false);

            if (result == RefreshResult.Ok)
            {
                await ThreadHelper.SwitchToMainThreadAsync();
                Summary = summary;
                SelectedAvatar = Summary?.Avatars.FirstOrDefault();
            }
            else
            {
                switch (result)
                {
                    case RefreshResult.APIUnavailable:
                        infoBarService.Warning("角色信息服务当前不可用");
                        break;
                    case RefreshResult.ShowcaseNotOpen:
                        infoBarService.Warning("角色橱窗尚未开启，请前往游戏操作后重试");
                        break;
                }
            }
        }
        catch (OperationCanceledException)
        {
        }
    }
}