// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using Snap.Hutao.Web.Response;
using System.Net.Http;
using System.Net.Http.Json;

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

/// <summary>
/// 用户信息客户端
/// </summary>
[HttpClient(HttpClientConfigration.XRpc)]
internal class UserClient
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions jsonSerializerOptions;

    /// <summary>
    /// 构造一个新的用户信息客户端
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    /// <param name="jsonSerializerOptions">Json序列化选项</param>
    public UserClient(HttpClient httpClient, JsonSerializerOptions jsonSerializerOptions)
    {
        this.httpClient = httpClient;
        this.jsonSerializerOptions = jsonSerializerOptions;
    }

    /// <summary>
    /// 获取当前用户详细信息
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>详细信息</returns>
    public async Task<UserInfo?> GetUserFullInfoAsync(Model.Binding.User user, CancellationToken token = default)
    {
        Response<UserFullInfoWrapper>? resp = await httpClient
            .SetUser(user)
            .GetFromJsonAsync<Response<UserFullInfoWrapper>>(ApiEndpoints.UserFullInfo, jsonSerializerOptions, token)
            .ConfigureAwait(false);

        return resp?.Data?.UserInfo;
    }

    /// <summary>
    /// 获取其他用户详细信息
    /// </summary>
    /// <param name="user">当前用户</param>
    /// <param name="uid">米游社Uid</param>
    /// <param name="token">取消令牌</param>
    /// <returns>详细信息</returns>
    public async Task<UserInfo?> GetUserFullInfoAsync(Model.Binding.User user, string uid, CancellationToken token = default)
    {
        Response<UserFullInfoWrapper>? resp = await httpClient
            .SetUser(user)
            .GetFromJsonAsync<Response<UserFullInfoWrapper>>(ApiEndpoints.UserFullInfoQuery(uid), jsonSerializerOptions, token)
            .ConfigureAwait(false);

        return resp?.Data?.UserInfo;
    }
}