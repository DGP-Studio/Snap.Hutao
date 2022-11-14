using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Snap.Hutao.Web.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Snap.Hutao.Web.Hoyolab.App.Account;

/// <summary>
/// 账户客户端
/// </summary>
[HttpClient(HttpClientConfigration.XRpc)]
internal class AccountClient
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<AccountClient> logger;

    /// <summary>
    /// 构造一个新的账户客户端
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    /// <param name="options">选项</param>
    /// <param name="logger">日志器</param>
    public AccountClient(HttpClient httpClient, JsonSerializerOptions options, ILogger<AccountClient> logger)
    {
        this.httpClient = httpClient;
        this.options = options;
        this.logger = logger;
    }

    /// <summary>
    /// 异步生成祈愿验证密钥
    /// 需要stoken
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="data">提交数据</param>
    /// <param name="token">取消令牌</param>
    /// <returns>用户角色信息</returns>
    [ApiInformation(Cookie = CookieType.Stoken, Salt = SaltType.K2)]
    public async Task<GameAuthKey?> GenerateAuthenticationKeyAsync(User user, GenAuthKeyData data, CancellationToken token = default)
    {
        Response<GameAuthKey>? resp = await httpClient
            .SetUser(user, CookieType.Stoken)
            .SetReferer("https://app.mihoyo.com")
            .UsingDynamicSecret1(SaltType.K2)
            .TryCatchPostAsJsonAsync<GenAuthKeyData, Response<GameAuthKey>>(ApiEndpoints.AppAccountGenAuthKey, data, options, logger, token)
            .ConfigureAwait(false);

        return resp?.Data;
    }

    /// <summary>
    /// 异步获取 Ltoken
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>Ltoken</returns>
    [ApiInformation(Cookie = CookieType.Stoken, Salt = SaltType.PROD)]
    public async Task<string?> GetLTokenBySTokenAsync(User user, CancellationToken token)
    {
        Response<LtokenWrapper>? resp = await httpClient
            .SetUser(user, CookieType.Stoken)
            .UsingDynamicSecret2(SaltType.PROD, options, ApiEndpoints.AppAccountGetLTokenBySToken)
            .TryCatchGetFromJsonAsync<Response<LtokenWrapper>>(token)
            .ConfigureAwait(false);

        return resp?.Data?.Ltoken;
    }
}
