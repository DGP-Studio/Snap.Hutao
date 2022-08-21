// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Binding;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Snap.Hutao.Web.Response;
using System.Net.Http;
using System.Net.Http.Json;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Event.BbsSignReward;

/// <summary>
/// 签到客户端
/// </summary>
[HttpClient(HttpClientConfigration.XRpc)]
internal class SignClient
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;

    /// <summary>
    /// 构造一个新的签到客户端
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    /// <param name="options">选项</param>
    public SignClient(HttpClient httpClient, JsonSerializerOptions options)
    {
        this.httpClient = httpClient;
        this.options = options;
    }

    /// <summary>
    /// 异步获取签到信息
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="role">角色</param>
    /// <param name="token">取消令牌</param>
    /// <returns>签到信息</returns>
    public async Task<SignInRewardInfo?> GetInfoAsync(User user, UserGameRole role, CancellationToken token = default)
    {
        Response<SignInRewardInfo>? resp = await httpClient
            .SetUser(user)
            .UsingDynamicSecret()
            .GetFromJsonAsync<Response<SignInRewardInfo>>(ApiEndpoints.SignInRewardInfo((PlayerUid)role), options, token)
            .ConfigureAwait(false);

        return resp?.Data;
    }

    /// <summary>
    /// 异步获取签到信息
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="role">角色</param>
    /// <param name="token">取消令牌</param>
    /// <returns>签到信息</returns>
    public async Task<SignInRewardReSignInfo?> GetResignInfoAsync(User user, UserGameRole role, CancellationToken token = default)
    {
        Response<SignInRewardReSignInfo>? resp = await httpClient
            .SetUser(user)
            .UsingDynamicSecret()
            .GetFromJsonAsync<Response<SignInRewardReSignInfo>>(ApiEndpoints.SignInRewardResignInfo((PlayerUid)role), options, token)
            .ConfigureAwait(false);

        return resp?.Data;
    }

    /// <summary>
    /// 获取签到奖励
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>奖励信息</returns>
    public async Task<Reward?> GetRewardAsync(User user, CancellationToken token = default)
    {
        Response<Reward>? resp = await httpClient
            .SetUser(user)
            .GetFromJsonAsync<Response<Reward>>(ApiEndpoints.SignInRewardHome, options, token)
            .ConfigureAwait(false);

        return resp?.Data;
    }

    /// <summary>
    /// 补签
    /// </summary>
    /// <param name="userRole">用户角色</param>
    /// <param name="token">取消令牌</param>
    /// <returns>签到结果</returns>
    public Task<Response<SignInResult>?> ReSignAsync(UserRole userRole, CancellationToken token = default)
    {
        return ReSignAsync(userRole.User, userRole.Role, token);
    }

    /// <summary>
    /// 补签
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="role">角色</param>
    /// <param name="token">取消令牌</param>
    /// <returns>签到消息</returns>
    public async Task<Response<SignInResult>?> ReSignAsync(User user, UserGameRole role, CancellationToken token = default)
    {
        SignInData data = new((PlayerUid)role);

        HttpResponseMessage response = await httpClient
            .SetUser(user)
            .UsingDynamicSecret()
            .PostAsJsonAsync(ApiEndpoints.SignInRewardReSign, data, options, token)
            .ConfigureAwait(false);
        Response<SignInResult>? resp = await response.Content
            .ReadFromJsonAsync<Response<SignInResult>>(options, token)
            .ConfigureAwait(false);

        return resp;
    }

    /// <summary>
    /// 签到
    /// </summary>
    /// <param name="userRole">用户角色</param>
    /// <param name="token">取消令牌</param>
    /// <returns>签到结果</returns>
    public Task<Response<SignInResult>?> SignAsync(UserRole userRole, CancellationToken token = default)
    {
        return SignAsync(userRole.User, userRole.Role, token);
    }

    /// <summary>
    /// 签到
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="role">角色</param>
    /// <param name="token">取消令牌</param>
    /// <returns>签到结果</returns>
    public async Task<Response<SignInResult>?> SignAsync(User user, UserGameRole role, CancellationToken token = default)
    {
        HttpResponseMessage response = await httpClient
            .SetUser(user)
            .UsingDynamicSecret()
            .PostAsJsonAsync(ApiEndpoints.SignInRewardSign, new SignInData((PlayerUid)role), options, token)
            .ConfigureAwait(false);
        Response<SignInResult>? resp = await response.Content
            .ReadFromJsonAsync<Response<SignInResult>>(options, token)
            .ConfigureAwait(false);

        return resp;
    }
}