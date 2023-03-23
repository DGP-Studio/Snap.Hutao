// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Setting;
using Snap.Hutao.Web.Hutao;

namespace Snap.Hutao.Service.Hutao;

/// <summary>
/// 胡桃用户服务
/// </summary>
[Injection(InjectAs.Singleton, typeof(IHutaoUserService))]
internal sealed class HutaoUserService : IHutaoUserService, IHutaoUserServiceInitialization
{
    private readonly HomaPassportClient passportClient;
    private readonly TaskCompletionSource initializeCompletionSource = new();

    private bool isInitialized;

    /// <summary>
    /// 构造一个新的胡桃用户服务
    /// </summary>
    /// <param name="passportClient">通行证客户端</param>
    public HutaoUserService(HomaPassportClient passportClient)
    {
        this.passportClient = passportClient;
    }

    /// <summary>
    /// 用户名
    /// </summary>
    public string? UserName { get; private set; }

    /// <summary>
    /// 访问令牌
    /// </summary>
    public string? Token { get; private set; }

    /// <summary>
    /// 异步初始化
    /// </summary>
    /// <returns>任务</returns>
    public async ValueTask<bool> InitializeAsync()
    {
        await initializeCompletionSource.Task.ConfigureAwait(false);
        return isInitialized;
    }

    /// <inheritdoc/>
    public async Task InitializeInternalAsync(CancellationToken token = default)
    {
        string userName = LocalSetting.Get(SettingKeys.PassportUserName, string.Empty);
        string passport = LocalSetting.Get(SettingKeys.PassportPassword, string.Empty);

        if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(passport))
        {
            Web.Response.Response<string> response = await passportClient.LoginAsync(userName, passport, token).ConfigureAwait(false);

            if (response.IsOk())
            {
                Token = response.Data;
                UserName = userName;
                isInitialized = true;
            }
            else
            {
                UserName = SH.ViewServiceHutaoUserLoginFailHint;
            }
        }
        else
        {
            UserName = SH.ViewServiceHutaoUserLoginOrRegisterHint;
        }

        initializeCompletionSource.TrySetResult();
    }
}