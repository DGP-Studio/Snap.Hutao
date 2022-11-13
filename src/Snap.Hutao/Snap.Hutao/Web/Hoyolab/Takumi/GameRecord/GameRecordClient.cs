// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Annotation.HttpClient;
using Snap.Hutao.Extension;
using Snap.Hutao.Model.Entity;
using Snap.Hutao.Web.Hoyolab.Annotation;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;
using Snap.Hutao.Web.Response;
using System.Net.Http;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;

/// <summary>
/// 游戏记录提供器
/// </summary>
[HttpClient(HttpClientConfigration.XRpc)]
internal class GameRecordClient
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
    public GameRecordClient(HttpClient httpClient, JsonSerializerOptions options, ILogger<GameRecordClient> logger)
    {
        this.httpClient = httpClient;
        this.options = options;
        this.logger = logger;
    }

    /// <summary>
    /// 异步获取实时便笺
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="uid">查询uid</param>
    /// <param name="token">取消令牌</param>
    /// <returns>实时便笺</returns>
    [ApiInformation(Cookie = CookieType.Ltoken, Salt = SaltType.X4)]
    public async Task<DailyNote.DailyNote?> GetDailyNoteAsync(User user, PlayerUid uid, CancellationToken token = default)
    {
        Response<DailyNote.DailyNote>? resp = await httpClient
            .SetUser(user, CookieType.Ltoken)
            .UsingDynamicSecret2(SaltType.X4, options, ApiEndpoints.GameRecordDailyNote(uid.Value, uid.Region))
            .GetFromJsonAsync<Response<DailyNote.DailyNote>>(token)
            .ConfigureAwait(false);

        return resp?.Data;
    }

    /// <summary>
    /// 获取玩家基础信息
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="uid">uid</param>
    /// <param name="token">取消令牌</param>
    /// <returns>玩家的基础信息</returns>
    [ApiInformation(Cookie = CookieType.Ltoken, Salt = SaltType.X4)]
    public async Task<PlayerInfo?> GetPlayerInfoAsync(User user, PlayerUid uid, CancellationToken token = default)
    {
        Response<PlayerInfo>? resp = await httpClient
            .SetUser(user, CookieType.Ltoken)
            .UsingDynamicSecret2(SaltType.X4, options, ApiEndpoints.GameRecordIndex(uid.Value, uid.Region))
            .GetFromJsonAsync<Response<PlayerInfo>>(token)
            .ConfigureAwait(false);

        return resp?.Data;
    }

    /// <summary>
    /// 获取玩家深渊信息
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="uid">uid</param>
    /// <param name="schedule">1：当期，2：上期</param>
    /// <param name="token">取消令牌</param>
    /// <returns>深渊信息</returns>
    [ApiInformation(Cookie = CookieType.Ltoken, Salt = SaltType.X4)]
    public async Task<SpiralAbyss.SpiralAbyss?> GetSpiralAbyssAsync(User user, PlayerUid uid, SpiralAbyssSchedule schedule, CancellationToken token = default)
    {
        Response<SpiralAbyss.SpiralAbyss>? resp = await httpClient
            .SetUser(user, CookieType.Ltoken)
            .UsingDynamicSecret2(SaltType.X4, options, ApiEndpoints.GameRecordSpiralAbyss(schedule, uid))
            .GetFromJsonAsync<Response<SpiralAbyss.SpiralAbyss>>(token)
            .ConfigureAwait(false);

        return resp?.Data;
    }

    /// <summary>
    /// 异步获取角色基本信息
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="uid">uid</param>
    /// <param name="token">取消令牌</param>
    /// <returns>角色基本信息</returns>
    [ApiInformation(Cookie = CookieType.Ltoken, Salt = SaltType.X4)]
    public async Task<BasicRoleInfo?> GetRoleBasicInfoAsync(User user, PlayerUid uid, CancellationToken token = default)
    {
        Response<BasicRoleInfo>? resp = await httpClient
            .SetUser(user, CookieType.Ltoken)
            .UsingDynamicSecret2(SaltType.X4, options, ApiEndpoints.GameRecordRoleBasicInfo(uid))
            .TryCatchGetFromJsonAsync<Response<BasicRoleInfo>>(logger, token)
            .ConfigureAwait(false);

        return resp?.Data;
    }

    /// <summary>
    /// 获取玩家角色详细信息
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="uid">uid</param>
    /// <param name="playerInfo">玩家的基础信息</param>
    /// <param name="token">取消令牌</param>
    /// <returns>角色列表</returns>
    [ApiInformation(Cookie = CookieType.Ltoken, Salt = SaltType.X4)]
    public async Task<List<Character>> GetCharactersAsync(User user, PlayerUid uid, PlayerInfo playerInfo, CancellationToken token = default)
    {
        CharacterData data = new(uid, playerInfo.Avatars.Select(x => x.Id));

        Response<CharacterWrapper>? resp = await httpClient
            .SetUser(user, CookieType.Ltoken)
            .UsingDynamicSecret2(SaltType.X4, options, ApiEndpoints.GameRecordCharacter, data)
            .TryCatchPostAsJsonAsync<Response<CharacterWrapper>>(logger, token)
            .ConfigureAwait(false);

        return EnumerableExtension.EmptyIfNull(resp?.Data?.Avatars);
    }

    private class CharacterData
    {
        public CharacterData(PlayerUid uid, IEnumerable<int> characterIds)
        {
            CharacterIds = characterIds;
            Uid = uid.Value;
            Server = uid.Region;
        }

        [JsonPropertyName("character_ids")]
        public IEnumerable<int> CharacterIds { get; }

        [JsonPropertyName("role_id")]
        public string Uid { get; }

        [JsonPropertyName("server")]
        public string Server { get; }
    }
}