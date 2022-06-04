// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Service.Abstraction;
using Snap.Hutao.Web.Hoyolab.DynamicSecret;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;
using Snap.Hutao.Web.Request;
using Snap.Hutao.Web.Response;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;

/// <summary>
/// 游戏记录提供器
/// </summary>
[Injection(InjectAs.Transient)]
internal class GameRecordProvider
{
    private readonly IUserService userService;
    private readonly Requester requester;

    /// <summary>
    /// 构造一个新的游戏记录提供器
    /// </summary>
    /// <param name="userService">用户服务</param>
    /// <param name="requester">请求器</param>
    public GameRecordProvider(IUserService userService, Requester requester)
    {
        this.userService = userService;
        this.requester = requester;
    }

    /// <summary>
    /// 获取玩家基础信息
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="token">取消令牌</param>
    /// <returns>玩家的基础信息</returns>
    public async Task<PlayerInfo?> GetPlayerInfoAsync(PlayerUid uid, CancellationToken token)
    {
        string url = string.Format(ApiEndpoints.GameRecordIndex, uid.Value, uid.Region);

        Response<PlayerInfo>? resp = await PrepareRequester()
            .GetUsingDS2Async<PlayerInfo>(url, token)
            .ConfigureAwait(false);

        return resp?.Data;
    }

    /// <summary>
    /// 获取玩家深渊信息
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="schedule">1：当期，2：上期</param>
    /// <param name="token">取消令牌</param>
    /// <returns>深渊信息</returns>
    public async Task<SpiralAbyss.SpiralAbyss?> GetSpiralAbyssAsync(PlayerUid uid, SpiralAbyssSchedule schedule, CancellationToken token = default)
    {
        string url = string.Format(ApiEndpoints.SpiralAbyss, (int)schedule, uid.Value, uid.Region);

        Response<SpiralAbyss.SpiralAbyss>? resp = await PrepareRequester()
            .GetUsingDS2Async<SpiralAbyss.SpiralAbyss>(url, token)
            .ConfigureAwait(false);

        return resp?.Data;
    }

    /// <summary>
    /// 获取玩家角色详细信息
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="playerInfo">玩家的基础信息</param>
    /// <param name="cancellationToken">取消令牌</param>
    /// <returns>角色列表</returns>
    public async Task<List<Character>> GetCharactersAsync(PlayerUid uid, PlayerInfo playerInfo, CancellationToken cancellationToken = default)
    {
        CharacterData data = new(uid, playerInfo.Avatars.Select(x => x.Id));

        Response<CharacterWrapper>? resp = await PrepareRequester()
            .PostUsingDS2Async<CharacterWrapper>(ApiEndpoints.Character, data, cancellationToken)
            .ConfigureAwait(false);

        return resp?.Data?.Avatars ?? new();
    }

    private Requester PrepareRequester()
    {
        return requester
            .Reset()
            .SetAcceptJson()
            .AddHeader(RequestHeaders.AppVersion, DynamicSecretProvider2.AppVersion)
            .SetCommonUA()
            .AddHeader(RequestHeaders.ClientType, RequestOptions.DefaultClientType)
            .SetRequestWithHyperion()
            .SetUser(userService.CurrentUser);
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