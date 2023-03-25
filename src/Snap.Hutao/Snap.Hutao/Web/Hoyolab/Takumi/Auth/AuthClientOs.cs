using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Takumi.Auth;

/// <summary>
/// Hoyolab 授权客户端
/// </summary>
[HighQuality]
[UseDynamicSecret]
[HttpClient(HttpClientConfiguration.Default)]
internal sealed class AuthClientOs
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<BindingClient> logger;

    /// <summary>
    /// 构造一个新的 Hoyolab 授权客户端
    /// </summary>
    /// <param name="httpClient">Http客户端</param>
    /// <param name="options">Json序列化选项</param>
    /// <param name="logger">日志器</param>
    public AuthClientOs(HttpClient httpClient, JsonSerializerOptions options, ILogger<BindingClient> logger)
    {
        this.httpClient = httpClient;
        this.options = options;
        this.logger = logger;
    }

    /// <summary>
    /// 获取 MultiToken
    /// </summary>
    /// <param name="cookie">login cookie</param>
    /// <param name="token">取消令牌</param>
    /// <returns>包含token的字典</returns>
    public async Task<Response<ListWrapper<NameToken>>> GetMultiTokenByLoginTicketAsync(Cookie cookie, CancellationToken token)
    {
        string loginTicket = cookie["login_ticket"];
        string loginUid = cookie["login_uid"];

        Response<ListWrapper<NameToken>>? resp = await httpClient
            .TryCatchGetFromJsonAsync<Response<ListWrapper<NameToken>>>(ApiOsEndpoints.AuthMultiToken(loginTicket, loginUid), options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}
