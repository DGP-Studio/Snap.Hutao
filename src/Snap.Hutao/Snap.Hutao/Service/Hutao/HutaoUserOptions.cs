// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.Options;

namespace Snap.Hutao.Service.Hutao;

/// <summary>
/// 胡桃用户选项
/// </summary>
[Injection(InjectAs.Singleton)]
internal sealed class HutaoUserOptions : ObservableObject, IOptions<HutaoUserOptions>
{
    private string? userName = SH.ViewServiceHutaoUserLoginOrRegisterHint;
    private string? token;
    private bool isLoggedIn;
    private bool isHutaoCloudServiceAllowed;

    /// <summary>
    /// 用户名
    /// </summary>
    public string? UserName { get => userName; set => SetProperty(ref userName, value); }

    /// <summary>
    /// 真正的用户名
    /// </summary>
    public string? ActualUserName { get => IsLoggedIn ? null : UserName; }

    /// <summary>
    /// 访问令牌
    /// </summary>
    public string? Token { get => token; set => SetProperty(ref token, value); }

    /// <summary>
    /// 是否已登录
    /// </summary>
    public bool IsLoggedIn { get => isLoggedIn; set => SetProperty(ref isLoggedIn, value); }

    /// <summary>
    /// 胡桃云服务是否可用
    /// </summary>
    public bool IsCloudServiceAllowed { get => isHutaoCloudServiceAllowed; set => SetProperty(ref isHutaoCloudServiceAllowed, value); }

    /// <inheritdoc/>
    public HutaoUserOptions Value { get => this; }

    /// <summary>
    /// 登录
    /// </summary>
    /// <param name="userName">用户名</param>
    /// <param name="token">令牌</param>
    public void LoginSucceed(string userName, string? token)
    {
        UserName = userName;
        Token = token;
        IsLoggedIn = true;
    }

    /// <summary>
    /// 登录失败
    /// </summary>
    public void LoginFailed()
    {
        UserName = SH.ViewServiceHutaoUserLoginFailHint;
    }
}