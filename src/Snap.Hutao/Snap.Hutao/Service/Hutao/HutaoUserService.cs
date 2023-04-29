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
    private readonly TaskCompletionSource initializeCompletionSource = new();

    private readonly ITaskContext taskContext;
    private readonly HomaPassportClient passportClient;
    private readonly HutaoUserOptions options;

    private bool isInitialized;

    /// <summary>
    /// 构造一个新的胡桃用户服务
    /// </summary>
    /// <param name="taskContext">任务上下文</param>
    /// <param name="passportClient">通行证客户端</param>
    /// <param name="options">选项</param>
    public HutaoUserService(ITaskContext taskContext, HomaPassportClient passportClient, HutaoUserOptions options)
    {
        this.taskContext = taskContext;
        this.passportClient = passportClient;
        this.options = options;
    }

    /// <inheritdoc/>
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
                await taskContext.SwitchToMainThreadAsync();
                options.LoginSucceed(userName, response.Data);
                isInitialized = true;
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