// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.Options;
using Snap.Hutao.Web.Hutao;
using System.Text.RegularExpressions;

namespace Snap.Hutao.Service.Hutao;

/// <summary>
/// 胡桃用户选项
/// </summary>
[Injection(InjectAs.Singleton)]
internal sealed class HutaoUserOptions : ObservableObject, IOptions<HutaoUserOptions>
{
    private readonly TaskCompletionSource initializedTaskCompletionSource = new();
    private string? userName = SH.ViewServiceHutaoUserLoginOrRegisterHint;
    private string? token;
    private bool isLoggedIn;
    private bool isHutaoCloudServiceAllowed;
    private bool isLicensedDeveloper;
    private string? gachaLogExpireAt;
    private string? gachaLogExpireAtSlim;
    private bool isMaintainer;

    /// <summary>
    /// 用户名
    /// </summary>
    public string? UserName { get => userName; set => SetProperty(ref userName, value); }

    /// <summary>
    /// 真正的用户名
    /// </summary>
    public string? ActualUserName { get => IsLoggedIn ? UserName : null; }

    /// <summary>
    /// 是否已登录
    /// </summary>
    public bool IsLoggedIn { get => isLoggedIn; set => SetProperty(ref isLoggedIn, value); }

    /// <summary>
    /// 胡桃云服务是否可用
    /// </summary>
    public bool IsCloudServiceAllowed { get => isHutaoCloudServiceAllowed; set => SetProperty(ref isHutaoCloudServiceAllowed, value); }

    /// <summary>
    /// 是否为开发者
    /// </summary>
    public bool IsLicensedDeveloper { get => isLicensedDeveloper; set => SetProperty(ref isLicensedDeveloper, value); }

    public bool IsMaintainer { get => isMaintainer; set => SetProperty(ref isMaintainer, value); }

    /// <summary>
    /// 祈愿记录服务到期时间
    /// </summary>
    public string? GachaLogExpireAt { get => gachaLogExpireAt; set => SetProperty(ref gachaLogExpireAt, value); }

    public string? GachaLogExpireAtSlim { get => gachaLogExpireAtSlim; set => SetProperty(ref gachaLogExpireAtSlim, value); }

    /// <inheritdoc/>
    public HutaoUserOptions Value { get => this; }

    public async ValueTask<bool> PostLoginSucceedAsync(HomaPassportClient passportClient, ITaskContext taskContext, string username, string? token)
    {
        await taskContext.SwitchToMainThreadAsync();
        UserName = username;
        this.token = token;
        IsLoggedIn = true;
        initializedTaskCompletionSource.TrySetResult();

        await taskContext.SwitchToBackgroundAsync();
        Web.Response.Response<UserInfo> userInfoResponse = await passportClient.GetUserInfoAsync(default).ConfigureAwait(false);
        if (userInfoResponse.IsOk())
        {
            await taskContext.SwitchToMainThreadAsync();
            UpdateUserInfo(userInfoResponse.Data);
            return true;
        }

        return false;
    }

    public void LogoutOrUnregister()
    {
        UserName = null;
        token = null;
        IsLoggedIn = false;
        ClearUserInfo();
    }

    /// <summary>
    /// 登录失败
    /// </summary>
    public void LoginFailed()
    {
        UserName = SH.ViewServiceHutaoUserLoginFailHint;
        initializedTaskCompletionSource.TrySetResult();
    }

    public void SkipLogin()
    {
        initializedTaskCompletionSource.TrySetResult();
    }

    /// <summary>
    /// 刷新用户信息
    /// </summary>
    /// <param name="userInfo">用户信息</param>
    public void UpdateUserInfo(UserInfo userInfo)
    {
        IsLicensedDeveloper = userInfo.IsLicensedDeveloper;
        IsMaintainer = userInfo.IsMaintainer;
        GachaLogExpireAt = Regex.Unescape(SH.ServiceHutaoUserGachaLogExpiredAt).Format(userInfo.GachaLogExpireAt);
        GachaLogExpireAtSlim = $"{userInfo.GachaLogExpireAt:yyyy.MM.dd HH:mm:ss}";
        IsCloudServiceAllowed = IsLicensedDeveloper || userInfo.GachaLogExpireAt > DateTimeOffset.Now;
    }

    public async ValueTask<string?> GetTokenAsync()
    {
        await initializedTaskCompletionSource.Task.ConfigureAwait(false);
        return token;
    }

    private void ClearUserInfo()
    {
        IsLicensedDeveloper = false;
        IsMaintainer = false;
        GachaLogExpireAt = null;
        GachaLogExpireAtSlim = null;
        IsCloudServiceAllowed = false;
    }
}