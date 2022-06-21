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

    private static bool TryValidateCookie(IDictionary<string, string> map, [NotNullWhen(true)] out SortedDictionary<string, string>? filteredCookie)
    {
        int validFlag = 4;

        filteredCookie = new();

        // O(1) to validate cookie
        foreach ((string key, string value) in map)
        {
            if (key == "account_id")
            {
                validFlag--;
                filteredCookie[key] = value;
            }

            if (key == "cookie_token")
            {
                validFlag--;
                filteredCookie[key] = value;
            }

            if (key == "ltoken")
            {
                validFlag--;
                filteredCookie[key] = value;
            }

            if (key == "ltuid")
            {
                validFlag--;
                filteredCookie[key] = value;
            }
        }

        return validFlag == 0;
    }

    private async Task OpenUIAsync()
    {
        Users = await userService.GetInitializedUsersAsync();
        SelectedUser = Users.FirstOrDefault();
    }

    private async Task AddUserAsync(Flyout? flyout)
    {
        // hide the flyout, otherwise dialog can't open.
        flyout?.Hide();

        Result<bool, string> result = await new UserDialog().GetInputCookieAsync();

        // user confirms the input
        if (result.IsOk)
        {
            IDictionary<string, string> map = userService.ParseCookie(result.Value);

            if (TryValidateCookie(map, out SortedDictionary<string, string>? filteredCookie))
            {
                string simplifiedCookie = string.Join(';', filteredCookie.Select(kvp => $"{kvp.Key}={kvp.Value}"));
                User user = new() { Cookie = simplifiedCookie };

                if (!await userService.TryAddUserAsync(user))
                {
                    infoBarService.Warning("提供的Cookie无效！");
                }
            }
        }
    }
}