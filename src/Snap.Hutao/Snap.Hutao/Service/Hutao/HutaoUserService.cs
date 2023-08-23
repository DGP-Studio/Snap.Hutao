// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Setting;
using Snap.Hutao.Web.Hutao;
using Snap.Hutao.Web.Hutao.SpiralAbyss;

namespace Snap.Hutao.Service.Hutao;

/// <summary>
/// 胡桃用户服务
/// </summary>
[ConstructorGenerated]
[Injection(InjectAs.Singleton, typeof(IHutaoUserService))]
internal sealed partial class HutaoUserService : IHutaoUserService, IHutaoUserServiceInitialization
{
    private readonly TaskCompletionSource initializeCompletionSource = new();

    private readonly HomaPassportClient passportClient;
    private readonly ITaskContext taskContext;
    private readonly HutaoUserOptions options;

    private bool isInitialized;

    /// <inheritdoc/>
    public async ValueTask<bool> InitializeAsync()
    {
        await initializeCompletionSource.Task.ConfigureAwait(false);
        return isInitialized;
    }

    /// <inheritdoc/>
    public async ValueTask InitializeInternalAsync(CancellationToken token = default)
    {
        string userName = LocalSetting.Get(SettingKeys.PassportUserName, string.Empty);
        string passport = LocalSetting.Get(SettingKeys.PassportPassword, string.Empty);

        if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(passport))
        {
            options.SkipLogin();
        }
        else
        {
            Web.Response.Response<string> response = await passportClient.LoginAsync(userName, passport, token).ConfigureAwait(false);

            if (response.IsOk())
            {
                await taskContext.SwitchToMainThreadAsync();
                options.LoginSucceed(userName, response.Data);

                await taskContext.SwitchToBackgroundAsync();
                Web.Response.Response<UserInfo> userInfoResponse = await passportClient.GetUserInfoAsync(response.Data, token).ConfigureAwait(false);
                if (userInfoResponse.IsOk())
                {
                    await taskContext.SwitchToMainThreadAsync();
                    options.UpdateUserInfo(userInfoResponse.Data);
                    isInitialized = true;
                }
            }
            else
            {
                await taskContext.SwitchToMainThreadAsync();
                options.LoginFailed();
            }
        }

        initializeCompletionSource.TrySetResult();
    }
}