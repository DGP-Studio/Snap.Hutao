using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using Snap.Hutao.Web.Response;
using System.Net.Http;
using System.Net.Http.Json;
using Windows.ApplicationModel.Contacts;

namespace Snap.Hutao.Web.Hoyolab.Passport;

/// <summary>
/// 通行证客户端 XRPC 版
/// </summary>
[HighQuality]
[UseDynamicSecret]
[HttpClient(HttpClientConfigration.XRpc3)]
internal sealed class PassportClientOs
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<PassportClient> logger;

    /// <summary>
    /// 构造一个新的国际服通行证客户端
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    /// <param name="options">json序列化选项</param>
    /// <param name="logger">日志器</param>
    public PassportClientOs(HttpClient httpClient, JsonSerializerOptions options, ILogger<PassportClient> logger)
    {
        this.httpClient = httpClient;
        this.options = options;
        this.logger = logger;
    }

    /// <summary>
    /// 异步获取 CookieToken
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>cookie token</returns>
    [ApiInformation(Cookie = CookieType.SToken, Salt = SaltType.None)]
    public async Task<Response<UidCookieToken>> GetCookieAccountInfoBySTokenAsync(User user, CancellationToken token = default)
    {
        Response<UidCookieToken>? resp = null;

        if (user.SToken == null)
        {
            return Response.Response.DefaultIfNull(resp);
        }

        string stoken = user.SToken.GetValueOrDefault(Cookie.STOKEN)!;

        // Post json with stoken and uid (stuid/ltuid)
        StokenData data = new(stoken, user.Aid!);
        resp = await httpClient
            .SetUser(user, CookieType.SToken)
            .TryCatchPostAsJsonAsync<StokenData, Response<UidCookieToken>>(ApiOsEndpoints.AccountGetCookieTokenBySToken, data, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 异步获取 Ltoken
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="token">取消令牌</param>
    /// <returns>uid 与 cookie token</returns>
    [ApiInformation(Cookie = CookieType.SToken, Salt = SaltType.None)]
    public async Task<Response<LtokenWrapper>> GetLtokenBySTokenAsync(User user, CancellationToken token)
    {
        Response<LtokenWrapper>? resp = null;

        if (user.SToken == null)
        {
            return Response.Response.DefaultIfNull(resp);
        }

        string stoken = user.SToken.GetValueOrDefault(Cookie.STOKEN)!;

        // Post json with stoken and uid (stuid/ltuid)
        StokenData data = new(stoken, user.Aid!);
        resp = await httpClient
            .SetUser(user, CookieType.SToken)
            .TryCatchPostAsJsonAsync<StokenData, Response<LtokenWrapper>>(ApiOsEndpoints.AccountGetLtokenByStoken, data, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    private class StokenData
    {
        public StokenData(string stoken, string uid)
        {
            Stoken = stoken;
            Uid = uid;
        }

        [JsonPropertyName("stoken")]
        public string Stoken { get; set; }

        [JsonPropertyName("uid")]
        public string Uid { get; set; }
    }
}
