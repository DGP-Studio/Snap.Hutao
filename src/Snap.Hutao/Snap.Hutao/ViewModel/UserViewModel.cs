// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.UI.Xaml.Controls;
using Snap.Hutao.Core.Threading;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.View.Dialog;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 用户视图模型
/// </summary>
[Injection(InjectAs.Transient)]
internal class UserViewModel : ObservableObject
{
    private readonly IUserService userService;
    private readonly IInfoBarService infoBarService;
    private readonly ICommand removeUserCommandCache;

    private User? selectedUser;
    private ObservableCollection<User>? userInfos;

    /// <summary>
    /// 构造一个新的用户视图模型
    /// </summary>
    /// <param name="userService">用户服务</param>
    /// <param name="infoBarService">信息条服务</param>
    /// <param name="asyncRelayCommandFactory">异步命令工厂</param>
    public UserViewModel(IUserService userService, IInfoBarService infoBarService, IAsyncRelayCommandFactory asyncRelayCommandFactory)
    {
        this.userService = userService;
        this.infoBarService = infoBarService;

        OpenUICommand = asyncRelayCommandFactory.Create(OpenUIAsync);
        AddUserCommand = asyncRelayCommandFactory.Create<Flyout>(AddUserAsync);

        removeUserCommandCache = asyncRelayCommandFactory.Create<User>(RemoveUserAsync);
    }

    /// <summary>
    /// 当前选择的用户信息
    /// </summary>
    public User? SelectedUser
    {
        get => selectedUser;
        set
        {
            if (SetProperty(ref selectedUser, value))
            {
                userService.CurrentUser = value;
            }
        }
    }

    /// <summary>
    /// 用户信息集合
    /// </summary>
    public ObservableCollection<User>? Users { get => userInfos; set => SetProperty(ref userInfos, value); }

    /// <summary>
    /// 打开界面命令
    /// </summary>
    public ICommand OpenUICommand { get; }

    /// <summary>
    /// 添加用户命令
    /// </summary>
    public ICommand AddUserCommand { get; }

    private static bool TryValidateCookie(IDictionary<string, string> map, [NotNullWhen(true)] out IDictionary<string, string>? filteredCookie)
    {
        int validFlag = 4;

        filteredCookie = new SortedDictionary<string, string>();

        // O(1) to validate cookie
        foreach ((string key, string value) in map)
        {
            if (key == "account_id" || key == "cookie_token" || key == "ltoken" || key == "ltuid")
            {
                validFlag--;
                filteredCookie[key] = value;
            }
        }

        if (validFlag == 0)
        {
            return true;
        }
        else
        {
            filteredCookie = null;
            return false;
        }
    }

    private async Task OpenUIAsync()
    {
        Users = await userService.GetInitializedUsersAsync(removeUserCommandCache);
        SelectedUser = userService.CurrentUser;
    }

    private async Task AddUserAsync(Flyout? flyout)
    {
        // hide the flyout, otherwise dialog can't open.
        flyout?.Hide();

        Result<bool, string> result = await new UserDialog().GetInputCookieAsync();

        // user confirms the input
        if (result.IsOk)
        {
            IDictionary<string, string> cookieMap = userService.ParseCookie(result.Value);

            if (TryValidateCookie(cookieMap, out IDictionary<string, string>? filteredCookie))
            {
                string simplifiedCookie = string.Join(';', filteredCookie.Select(kvp => $"{kvp.Key}={kvp.Value}"));
                User user = new()
                {
                    Cookie = simplifiedCookie,
                    RemoveCommand = removeUserCommandCache,
                };

                switch (await userService.TryAddUserAsync(user))
                {
                    case UserAddResult.Ok:
                        infoBarService.Success($"用户 [{user.UserInfo!.Nickname}] 添加成功");
                        break;
                    case UserAddResult.AlreadyExists:
                        infoBarService.Information($"用户 [{user.UserInfo!.Nickname}] 已经存在");
                        break;
                    case UserAddResult.InitializeFailed:
                        infoBarService.Warning("此Cookie无法获取用户信息，请重新输入");
                        break;
                }
            }
            else
            {
                infoBarService.Warning("提供的字符串并不是有效的Cookie，请重新输入");
            }
        }
    }

    private async Task RemoveUserAsync(User? user)
    {
        if (!User.IsNone(user))
        {
            await userService.RemoveUserAsync(user);
            infoBarService.Success($"用户 [{user.UserInfo!.Nickname}] 成功移除");
        }
    }
}