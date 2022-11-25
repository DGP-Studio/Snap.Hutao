// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Snap.Hutao.Core.IO.DataTransfer;
using Snap.Hutao.Extension;
using Snap.Hutao.Factory.Abstraction;
using Snap.Hutao.Model.Binding.User;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Service.Navigation;
using Snap.Hutao.Service.User;
using Snap.Hutao.View.Dialog;
using Snap.Hutao.View.Page;
using Snap.Hutao.Web.Hoyolab;
using System.Collections.ObjectModel;
using System.Text;

namespace Snap.Hutao.ViewModel;

/// <summary>
/// 用户视图模型
/// </summary>
[Injection(InjectAs.Singleton)]
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
        LoginMihoyoUserCommand = new RelayCommand(LoginMihoyoUser);
        RemoveUserCommand = asyncRelayCommandFactory.Create<User>(RemoveUserAsync);
        CopyCookieCommand = new RelayCommand<User>(CopyCookie);
        ShowSignInWebViewDialogCommand = asyncRelayCommandFactory.Create(ShowSignInWebViewDialogAsync);
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
                userService.Current = value;
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
    /// 登录米游社命令
    /// </summary>
    public ICommand LoginMihoyoUserCommand { get; }

    public ICommand ShowSignInWebViewDialogCommand { get; }

    /// <summary>
    /// 移除用户命令
    /// </summary>
    public ICommand RemoveUserCommand { get; }

    /// <summary>
    /// 复制Cookie命令
    /// </summary>
    public ICommand CopyCookieCommand { get; }

    private async Task OpenUIAsync()
    {
        Users = await userService.GetUserCollectionAsync().ConfigureAwait(true);
        SelectedUser = userService.Current;
    }

    private async Task AddUserAsync()
    {
        // Get cookie from user input
        MainWindow mainWindow = Ioc.Default.GetRequiredService<MainWindow>();
        ValueResult<bool, string> result = await new UserDialog(mainWindow).GetInputCookieAsync().ConfigureAwait(false);

        // User confirms the input
        if (result.IsOk)
        {
            Cookie cookie = Cookie.Parse(result.Value);

            (UserOptionResult optionResult, string uid) = await userService.ProcessInputCookieAsync(cookie).ConfigureAwait(false);

            switch (optionResult)
            {
                case UserOptionResult.Added:
                    infoBarService.Success($"用户 [{uid}] 添加成功");
                    break;
                case UserOptionResult.Incomplete:
                    infoBarService.Information($"此 Cookie 不完整，操作失败");
                    break;
                case UserOptionResult.Invalid:
                    infoBarService.Information($"此 Cookie 无效，操作失败");
                    break;
                case UserOptionResult.Updated:
                    infoBarService.Success($"用户 [{uid}] 更新成功");
                    break;
                default:
                    throw Must.NeverHappen();
            }
        }
    }

    private void LoginMihoyoUser()
    {
        Ioc.Default.GetRequiredService<INavigationService>().Navigate<LoginMihoyoUserPage>(INavigationAwaiter.Default);
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
        try
        {
            string cookieString = new StringBuilder()
                .Append(user.Stoken)
                .AppendIf(user.Stoken != null, ';')
                .Append(user.Ltoken)
                .AppendIf(user.Ltoken != null, ';')
                .Append(user.CookieToken)
                .ToString();

            Clipboard.SetText(cookieString);
            infoBarService.Success($"{user.UserInfo!.Nickname} 的 Cookie 复制成功");
        }
        catch (Exception e)
        {
            infoBarService.Error(e);
        }
    }

    private async Task ShowSignInWebViewDialogAsync()
    {
        MainWindow mainWindow = Ioc.Default.GetRequiredService<MainWindow>();
        await new SignInWebViewDialog(mainWindow).ShowAsync();
    }
}