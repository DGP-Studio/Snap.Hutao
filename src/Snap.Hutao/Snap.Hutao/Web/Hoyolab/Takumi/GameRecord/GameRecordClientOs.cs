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
    /// 异步获取实时便笺
    /// </summary>
    /// <param name="userAndUid">用户与角色</param>
    /// <param name="token">取消令牌</param>
    /// <returns>实时便笺</returns>
    [ApiInformation(Cookie = CookieType.Cookie, Salt = SaltType.OS)]
    public async Task<Response<DailyNote.DailyNote>> GetDailyNoteAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        Response<DailyNote.DailyNote>? resp = await httpClient
            .SetUser(userAndUid.User, CookieType.Cookie)
            .UseDynamicSecret(DynamicSecretVersion.Gen1, SaltType.OS, false)
            .TryCatchGetFromJsonAsync<Response<DailyNote.DailyNote>>(ApiOsEndpoints.GameRecordDailyNote(userAndUid.Uid.Value), options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 获取玩家基础信息
    /// </summary>
    /// <param name="userAndUid">用户与角色</param>
    /// <param name="token">取消令牌</param>
    /// <returns>玩家的基础信息</returns>
    [ApiInformation(Cookie = CookieType.Ltoken, Salt = SaltType.OS)]
    public async Task<Response<PlayerInfo>> GetPlayerInfoAsync(UserAndUid userAndUid, CancellationToken token = default)
    {
        Response<PlayerInfo>? resp = await httpClient
            .SetUser(userAndUid.User, CookieType.Cookie)
            .UseDynamicSecret(DynamicSecretVersion.Gen1, SaltType.OS, false)
            .TryCatchGetFromJsonAsync<Response<PlayerInfo>>(ApiOsEndpoints.GameRecordIndex(userAndUid.Uid), options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
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
        Response<SpiralAbyss.SpiralAbyss>? resp = await httpClient
            .SetUser(userAndUid.User, CookieType.Cookie)
            .UseDynamicSecret(DynamicSecretVersion.Gen1, SaltType.OS, false)
            .TryCatchGetFromJsonAsync<Response<SpiralAbyss.SpiralAbyss>>(ApiOsEndpoints.GameRecordSpiralAbyss(schedule, userAndUid.Uid), options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    /// <summary>
    /// 获取玩家角色详细信息
    /// </summary>
    /// <param name="userAndUid">用户与角色</param>
    /// <param name="playerInfo">玩家的基础信息</param>
    /// <param name="token">取消令牌</param>
    /// <returns>角色列表</returns>
    [ApiInformation(Cookie = CookieType.Ltoken, Salt = SaltType.X4)]
    public async Task<Response<CharacterWrapper>> GetCharactersAsync(UserAndUid userAndUid, PlayerInfo playerInfo, CancellationToken token = default)
    {
        CharacterData data = new(userAndUid.Uid, playerInfo.Avatars.Select(x => x.Id));

        Response<CharacterWrapper>? resp = await httpClient
            .SetUser(userAndUid.User, CookieType.Cookie)
            .UseDynamicSecret(DynamicSecretVersion.Gen1, SaltType.OS, false)
            .TryCatchPostAsJsonAsync<CharacterData, Response<CharacterWrapper>>(ApiOsEndpoints.GameRecordCharacter, data, options, logger, token)
            .ConfigureAwait(false);

        return Response.Response.DefaultIfNull(resp);
    }

    private class CharacterData
    {
        public CharacterData(PlayerUid uid, IEnumerable<AvatarId> characterIds)
        {
            CharacterIds = characterIds;
            Uid = uid.Value;
            Server = uid.Region;
        }

        [JsonPropertyName("character_ids")]
        public IEnumerable<AvatarId> CharacterIds { get; }

        [JsonPropertyName("role_id")]
        public string Uid { get; }

        [JsonPropertyName("server")]
        public string Server { get; }
    }
}
