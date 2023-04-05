// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Bbs.User;

/// <summary>
/// 用户信息客户端 Hoyolab版
/// </summary>
[UseDynamicSecret]
[HttpClient(HttpClientConfiguration.Default)]
[Injection(InjectAs.Transient, typeof(IUserClient))]
internal sealed class UserClientOversea : IUserClient
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<UserClientOversea> logger;

    /// <summary>
    /// 构造一个新的用户信息客户端
    /// </summary>
    /// <param name="httpClientFactory">http客户端工厂</param>
    /// <param name="options">Json序列化选项</param>
    /// <param name="logger">日志器</param>
    public UserClientOversea(IHttpClientFactory httpClientFactory, JsonSerializerOptions options, ILogger<UserClientOversea> logger)
    {
        httpClient = httpClientFactory.CreateClient(nameof(UserClientOversea));

        this.options = options;
        this.logger = logger;
    }

    /// <inheritdoc/>
    public bool IsOversea
    {
        get => true;
    }

    /// <summary>
    /// 获取当前用户详细信息，使用 LToken
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>详细信息</returns>
    [ApiInformation(Cookie = CookieType.LToken, Salt = SaltType.None)]
    public async Task<Response<UserFullInfoWrapper>> GetUserFullInfoAsync(Model.Entity.User user, CancellationToken token = default)
    {
        Response<UserFullInfoWrapper>? resp = await httpClient
            .SetUser(user, CookieType.LToken)
            .TryCatchGetFromJsonAsync<Response<UserFullInfoWrapper>>(ApiOsEndpoints.UserFullInfoQuery(user.Aid!), options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}