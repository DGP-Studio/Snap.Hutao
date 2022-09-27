// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Snap.Hutao.Core.Threading;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model.Binding;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.View.Dialog;
using Snap.Hutao.Web.Hoyolab;
using System.Collections.ObjectModel;
using Windows.ApplicationModel.DataTransfer;

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
    private ObservableCollection<User>? users;

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
        AddUserCommand = asyncRelayCommandFactory.Create(AddUserAsync);
        UpgradeToStokenCommand = asyncRelayCommandFactory.Create(UpgradeByLoginTicketAsync);
        RemoveUserCommand = asyncRelayCommandFactory.Create<User>(RemoveUserAsync);
        CopyCookieCommand = new RelayCommand<User>(CopyCookie);
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
    public ObservableCollection<User>? Users { get => users; set => SetProperty(ref users, value); }

    /// <summary>
    /// 打开界面命令
    /// </summary>
    public ICommand OpenUICommand { get; }

    /// <summary>
    /// 添加用户命令
    /// </summary>
    public ICommand AddUserCommand { get; }

    /// <summary>
    /// 升级到Stoken命令
    /// </summary>
    public ICommand UpgradeToStokenCommand { get; }

    /// <summary>
    /// 移除用户命令
    /// </summary>
    public ICommand RemoveUserCommand { get; }

    /// <summary>
    /// 复制Cookie命令
    /// </summary>
    public ICommand CopyCookieCommand { get; }

    private static (bool Valid, bool Upgrade) TryValidateCookie(IDictionary<string, string> map, out IDictionary<string, string> cookie)
    {
        int validFlag = 4;
        int stokenFlag = 2;

        cookie = new SortedDictionary<string, string>();

        foreach ((string key, string value) in map)
        {
            switch (key)
            {
                case CookieKeys.COOKIE_TOKEN:
                case CookieKeys.ACCOUNT_ID:
                case CookieKeys.LTOKEN:
                case CookieKeys.LTUID:
                    {
                        validFlag--;
                        cookie.Add(key, value);
                        break;
                    }

                case CookieKeys.STOKEN:
                case CookieKeys.STUID:
                    {
                        stokenFlag--;
                        cookie.Add(key, value);
                        break;
                    }

                case CookieKeys.LOGIN_TICKET:
                case CookieKeys.LOGIN_UID:
                    {
                        cookie.Add(key, value);
                        break;
                    }
            }
        }

        return (validFlag == 0, stokenFlag == 0);
    }

    private async Task OpenUIAsync()
    {
        Users = await userService.GetUserCollectionAsync().ConfigureAwait(true);
        SelectedUser = userService.CurrentUser;
    }

    private async Task AddUserAsync()
    {
        // Get cookie from user input
        MainWindow mainWindow = Ioc.Default.GetRequiredService<MainWindow>();
        ValueResult<bool, string> result = await new UserDialog(mainWindow).GetInputCookieAsync().ConfigureAwait(false);

        // User confirms the input
        if (result.IsOk)
        {
            (bool valid, bool upgradable) = TryValidateCookie(User.MapCookie(result.Value), out IDictionary<string, string> cookie);
            if (valid)
            {
                if (await userService.CreateUserAsync(cookie).ConfigureAwait(false) is User user)
                {
                    switch (await userService.TryAddUserAsync(user, cookie[CookieKeys.ACCOUNT_ID]).ConfigureAwait(false))
                    {
                        case UserAddResult.Added:
                            infoBarService.Success($"用户 [{user.UserInfo!.Nickname}] 添加成功");
                            break;
                        case UserAddResult.Updated:
                            infoBarService.Success($"用户 [{user.UserInfo!.Nickname}] 更新成功");
                            break;
                        case UserAddResult.AlreadyExists:
                            infoBarService.Information($"用户 [{user.UserInfo!.Nickname}] 已经存在");
                            break;
                        default:
                            throw Must.NeverHappen();
                    }
                }
                else
                {
                    infoBarService.Warning("此 Cookie 无法获取用户信息，请重新输入");
                }
            }
            else
            {
                if (upgradable)
                {
                    (bool success, string nickname) = await userService.TryUpgradeUserByStokenAsync(cookie).ConfigureAwait(false);

                    if (success)
                    {
                        infoBarService.Information($"用户 [{nickname}] 的 Stoken 更新成功");
                    }
                    else
                    {
                        infoBarService.Warning($"未找到匹配的可升级用户");
                    }
                }
                else
                {
                    infoBarService.Warning("提供的文本不是正确的 Cookie ，请重新输入");
                }
            }
        }
    }

    private async Task UpgradeByLoginTicketAsync()
    {
        // Get cookie from user input
        MainWindow mainWindow = Ioc.Default.GetRequiredService<MainWindow>();
        (bool isOk, IDictionary<string, string> addition) = await new UserAutoCookieDialog(mainWindow).GetInputCookieAsync().ConfigureAwait(false);

        // User confirms the input
        if (isOk)
        {
            (bool isUpgraded, string nickname) = await userService.TryUpgradeUserByLoginTicketAsync(addition).ConfigureAwait(false);
            if (isUpgraded)
            {
                infoBarService.Information($"用户 [{nickname}] 的 Cookie 已成功添加 Stoken");
            }
            else
            {
                infoBarService.Warning("请先添加对应用户的米游社Cookie");
            }
        }
    }

    private async Task RemoveUserAsync(User? user)
    {
        Verify.Operation(user != null, "待删除的用户不应为 null");
        await userService.RemoveUserAsync(user).ConfigureAwait(false);
        infoBarService.Success($"用户 [{user.UserInfo?.Nickname}] 成功移除");
    }

    private void CopyCookie(User? user)
    {
        Verify.Operation(user != null, "待复制 Cookie 的用户不应为 null");

        IInfoBarService infoBarService = Ioc.Default.GetRequiredService<IInfoBarService>();
        try
        {
            DataPackage content = new();
            content.SetText(Must.NotNull(user.Cookie!));
            Clipboard.SetContent(content);
            Clipboard.Flush();
            infoBarService.Success($"{user.UserInfo!.Nickname} 的 Cookie 复制成功");
        }
        catch (Exception e)
        {
            infoBarService.Error(e);
        }
    }
}