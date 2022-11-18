// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Auth;

/// <summary>
/// 授权客户端
/// </summary>
[UseDynamicSecret]
[HttpClient(HttpClientConfigration.Default)]
internal class AuthClient
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<BindingClient> logger;

    /// <summary>
    /// 构造一个新的授权客户端
    /// </summary>
    /// <param name="httpClient">Http客户端</param>
    /// <param name="options">Json序列化选项</param>
    /// <param name="logger">日志器</param>
    public AuthClient(HttpClient httpClient, JsonSerializerOptions options, ILogger<BindingClient> logger)
    {
        this.httpClient = httpClient;
        this.options = options;
        this.logger = logger;
    }

    /// <summary>
    /// 异步获取操作凭证
    /// </summary>
    /// <param name="action">操作</param>
    /// <param name="user">用户</param>
    /// <returns>操作凭证</returns>
    [ApiInformation(Cookie = CookieType.Stoken, Salt = SaltType.K2)]
    public async Task<string?> GetActionTicketByStokenAsync(string action, User user)
    {
        if (user.Stoken != null)
        {
            user.Stoken.TryGetValue(Cookie.STOKEN, out string? token);
            Response<ActionTicketWrapper>? resp = await httpClient
                .UseDynamicSecret(DynamicSecretVersion.Gen1, SaltType.K2, true)
                .TryCatchGetFromJsonAsync<Response<ActionTicketWrapper>>(ApiEndpoints.AuthActionTicket(action, token!, user.Aid!), options, logger)
                .ConfigureAwait(false);

            return resp?.Data?.Ticket;
        }

        return null;
    }
}