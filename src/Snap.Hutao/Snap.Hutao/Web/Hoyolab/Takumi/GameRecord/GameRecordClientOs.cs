using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Model.Binding.User;
using Snap.Hutao.Model.Primitive;
using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;

/// <summary>
/// Hoyoverse game record provider
/// </summary>
[HighQuality]
[UseDynamicSecret]
[HttpClient(HttpClientConfigration.XRpc3)]
[PrimaryHttpMessageHandler(UseCookies = false)]
internal sealed class GameRecordClientOs
{
    private readonly HttpClient httpClient;
    private readonly JsonSerializerOptions options;
    private readonly ILogger<GameRecordClient> logger;

    /// <summary>
    /// 构造一个新的游戏记录提供器
    /// </summary>
    /// <param name="httpClient">请求器</param>
    /// <param name="options">json序列化选项</param>
    /// <param name="logger">日志器</param>
    public GameRecordClientOs(HttpClient httpClient, JsonSerializerOptions options, ILogger<GameRecordClient> logger)
    {
        this.httpClient = httpClient;
        this.options = options;
        this.logger = logger;
    }

    /// <summary>
    /// 获取玩家深渊信息
    /// </summary>
    /// <param name="userAndUid">用户</param>
    /// <param name="schedule">1：当期，2：上期</param>
    /// <param name="token">取消令牌</param>
    /// <returns>深渊信息</returns>
    [ApiInformation(Cookie = CookieType.Cookie, Salt = SaltType.OS)]
    public async Task<Response<SpiralAbyss.SpiralAbyss>> GetSpiralAbyssAsync(UserAndUid userAndUid, SpiralAbyssSchedule schedule, CancellationToken token = default)
    {
        System.Net.Http.Headers.HttpRequestHeaders headers = httpClient.DefaultRequestHeaders;
        Response<SpiralAbyss.SpiralAbyss>? resp = await httpClient
            .SetUser(userAndUid.User, CookieType.Cookie)
            .UseDynamicSecret(DynamicSecretVersion.Gen1, SaltType.OS, false)
            .TryCatchGetFromJsonAsync<Response<SpiralAbyss.SpiralAbyss>>(ApiOsEndpoints.GameRecordSpiralAbyss(schedule, userAndUid.Uid), options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }
}
