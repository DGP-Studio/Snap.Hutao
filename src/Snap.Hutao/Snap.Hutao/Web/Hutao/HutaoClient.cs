// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Extension;
using Snap.Hutao.Web.Hoyolab.Takumi;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;
using Snap.Hutao.Web.Hutao.Model;
using Snap.Hutao.Web.Hutao.Model.Post;
using Snap.Hutao.Web.Response;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;

namespace Snap.Hutao.Web.Hutao;

/// <summary>
/// 胡桃API客户端
/// </summary>
[Injection(InjectAs.Transient)]
internal class HutaoClient : IAsyncInitializable
{
    private const string AuthAPIHost = "https://auth.snapgenshin.com";
    private const string HutaoAPI = "https://hutao-api.snapgenshin.com";

    private readonly HttpClient httpClient;
    private readonly GameRecordClient gameRecordClient;
    private readonly UserGameRoleClient userGameRoleClient;
    private readonly JsonSerializerOptions jsonSerializerOptions;

    private bool isInitialized = false;

    /// <summary>
    /// 构造一个新的胡桃API客户端
    /// </summary>
    /// <param name="httpClient">http客户端</param>
    /// <param name="gameRecordClient">游戏记录客户端</param>
    /// <param name="userGameRoleClient">用户游戏角色客户端</param>
    /// <param name="jsonSerializerOptions">json序列化选项</param>
    public HutaoClient(
        HttpClient httpClient,
        GameRecordClient gameRecordClient,
        UserGameRoleClient userGameRoleClient,
        JsonSerializerOptions jsonSerializerOptions)
    {
        this.httpClient = httpClient;
        this.gameRecordClient = gameRecordClient;
        this.userGameRoleClient = userGameRoleClient;
        this.jsonSerializerOptions = jsonSerializerOptions;
    }

    /// <inheritdoc/>
    public bool IsInitialized { get => isInitialized; }

    /// <inheritdoc/>
    public async Task<bool> InitializeAsync(CancellationToken token = default)
    {
        Auth auth = new(
            "08d9e212-0cb3-4d71-8ed7-003606da7b20",
            "7ueWgZGn53dDhrm8L5ZRw+YWfOeSWtgQmJWquRgaygw=");
        JsonSerializerOptions? option = Ioc.Default.GetService<JsonSerializerOptions>();

        HttpResponseMessage response = await httpClient
            .PostAsJsonAsync($"{AuthAPIHost}/Auth/Login", auth, option, token)
            .ConfigureAwait(false);
        Response<Token>? resp = await response.Content
            .ReadFromJsonAsync<Response<Token>>(option, token)
            .ConfigureAwait(false);

        httpClient.DefaultRequestHeaders.Authorization = new("Bearer", Must.NotNull(resp?.Data?.AccessToken!));
        isInitialized = true;
        return true;
    }

    /// <summary>
    /// 检查对应的uid当前是否上传了数据
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="token">取消令牌</param>
    /// <returns>当前是否上传了数据</returns>
    public async Task<bool> CheckPeriodRecordUploadedAsync(string uid, CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        Response<UploadStatus>? resp = await httpClient
            .GetFromJsonAsync<Response<UploadStatus>>($"{HutaoAPI}/Record/CheckRecord/{uid}", token)
            .ConfigureAwait(false);

        return resp is { Data: not null, Data.PeriodUploaded: true };
    }

    /// <summary>
    /// 异步获取总览数据
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>总览信息</returns>
    public async Task<Overview?> GetOverviewAsync(CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        Response<Overview>? resp = await httpClient
            .GetFromJsonAsync<Response<Overview>>($"{HutaoAPI}/Statistics/Overview", token)
            .ConfigureAwait(false);

        return resp?.Data;
    }

    /// <summary>
    /// 异步获取角色出场率
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色出场率</returns>
    public async Task<IEnumerable<AvatarParticipation>> GetAvatarParticipationsAsync(CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        Response<IEnumerable<AvatarParticipation>>? resp = await httpClient
            .GetFromJsonAsync<Response<IEnumerable<AvatarParticipation>>>($"{HutaoAPI}/Statistics/AvatarParticipation", token)
            .ConfigureAwait(false);

        return EnumerableExtensions.EmptyIfNull(resp?.Data);
    }

    /// <summary>
    /// 异步获取角色圣遗物搭配
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色圣遗物搭配</returns>
    public async Task<IEnumerable<AvatarReliquaryUsage>> GetAvatarReliquaryUsagesAsync(CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        Response<IEnumerable<AvatarReliquaryUsage>>? resp = await httpClient
            .GetFromJsonAsync<Response<IEnumerable<AvatarReliquaryUsage>>>($"{HutaoAPI}/Statistics/AvatarReliquaryUsage", token)
            .ConfigureAwait(false);

        return EnumerableExtensions.EmptyIfNull(resp?.Data);
    }

    /// <summary>
    /// 异步获取角色搭配数据
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色搭配数据</returns>
    public async Task<IEnumerable<TeamCollocation>> GetTeamCollocationsAsync(CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        Response<IEnumerable<TeamCollocation>>? resp = await httpClient
            .GetFromJsonAsync<Response<IEnumerable<TeamCollocation>>>($"{HutaoAPI}/Statistics/TeamCollocation", token)
            .ConfigureAwait(false);

        return EnumerableExtensions.EmptyIfNull(resp?.Data);
    }

    /// <summary>
    /// 异步获取角色武器搭配数据
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色武器搭配数据</returns>
    public async Task<IEnumerable<AvatarWeaponUsage>> GetAvatarWeaponUsagesAsync(CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        Response<IEnumerable<AvatarWeaponUsage>>? resp = await httpClient
            .GetFromJsonAsync<Response<IEnumerable<AvatarWeaponUsage>>>($"{HutaoAPI}/Statistics/AvatarWeaponUsage", token)
            .ConfigureAwait(false);

        return EnumerableExtensions.EmptyIfNull(resp?.Data);
    }

    /// <summary>
    /// 异步获取角色命座信息
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色图片列表</returns>
    public async Task<IEnumerable<AvatarConstellation>> GetAvatarConstellationsAsync(CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        Response<IEnumerable<AvatarConstellation>>? resp = await httpClient
            .GetFromJsonAsync<Response<IEnumerable<AvatarConstellation>>>($"{HutaoAPI}/Statistics/Constellation", token)
            .ConfigureAwait(false);

        return EnumerableExtensions.EmptyIfNull(resp?.Data);
    }

    /// <summary>
    /// 异步获取队伍出场次数
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>队伍出场列表</returns>
    public async Task<IEnumerable<TeamCombination>> GetTeamCombinationsAsync(CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        Response<IEnumerable<TeamCombination>>? resp = await httpClient
            .GetFromJsonAsync<Response<IEnumerable<TeamCombination>>>($"{HutaoAPI}/Statistics/TeamCombination", token)
            .ConfigureAwait(false);

        return EnumerableExtensions.EmptyIfNull(resp?.Data);
    }

    /// <summary>
    /// 异步获取队伍出场次数
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>队伍出场列表</returns>
    public async Task<IEnumerable<TeamCombination2>> GetTeamCombination2sAsync(CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        Response<IEnumerable<TeamCombination2>>? resp = await httpClient
            .GetFromJsonAsync<Response<IEnumerable<TeamCombination2>>>($"{HutaoAPI}/Statistics2/TeamCombination", token)
            .ConfigureAwait(false);

        return EnumerableExtensions.EmptyIfNull(resp?.Data);
    }

    /// <summary>
    /// 异步获取队伍出场次数
    /// </summary>
    /// <param name="floor">楼层</param>
    /// <param name="avatarIds">期望的角色，按期望出现顺序排序</param>
    /// <param name="token">取消令牌</param>
    /// <returns>队伍出场列表</returns>
    public async Task<IEnumerable<TeamCombination2>> GetRecommandedTeamCombination2sAsync(int floor, IEnumerable<string> avatarIds, CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        DesiredInfo desiredInfo = new(floor, avatarIds);

        HttpResponseMessage response = await httpClient
            .PostAsJsonAsync($"{HutaoAPI}/Statistics2/TeamRecommanded", desiredInfo, jsonSerializerOptions, token)
            .ConfigureAwait(false);

        Response<IEnumerable<TeamCombination2>>? resp = await response.Content
            .ReadFromJsonAsync<Response<IEnumerable<TeamCombination2>>>(jsonSerializerOptions, token)
            .ConfigureAwait(false);

        return EnumerableExtensions.EmptyIfNull(resp?.Data);
    }

    /// <summary>
    /// 异步获取角色图片列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色图片列表</returns>
    public async Task<IEnumerable<Item>> GetAvatarMapAsync(CancellationToken token = default)
    {
        Response<IEnumerable<Item>>? resp = await httpClient
            .GetFromJsonAsync<Response<IEnumerable<Item>>>($"{HutaoAPI}/GenshinItem/Avatars", token)
            .ConfigureAwait(false);

        return EnumerableExtensions.EmptyIfNull(resp?.Data);
    }

    /// <summary>
    /// 异步获取武器图片列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>武器图片列表</returns>
    public async Task<IEnumerable<Item>> GetWeaponMapAsync(CancellationToken token = default)
    {
        Response<IEnumerable<Item>>? resp = await httpClient
            .GetFromJsonAsync<Response<IEnumerable<Item>>>($"{HutaoAPI}/GenshinItem/Weapons", token)
            .ConfigureAwait(false);

        return EnumerableExtensions.EmptyIfNull(resp?.Data);
    }

    /// <summary>
    /// 异步获取圣遗物图片列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>圣遗物图片列表</returns>
    public async Task<IEnumerable<Item>> GetReliquaryMapAsync(CancellationToken token = default)
    {
        Response<IEnumerable<Item>>? resp = await httpClient
            .GetFromJsonAsync<Response<IEnumerable<Item>>>($"{HutaoAPI}/GenshinItem/Reliquaries", token)
            .ConfigureAwait(false);

        return EnumerableExtensions.EmptyIfNull(resp?.Data);
    }

    /// <summary>
    /// 异步获取角色的深渊记录
    /// </summary>
    /// <param name="role">角色</param>
    /// <param name="token">取消令牌</param>
    /// <returns>玩家记录</returns>
    public async Task<PlayerRecord> GetPlayerRecordAsync(UserGameRole role, CancellationToken token = default)
    {
        PlayerInfo? playerInfo = await gameRecordClient
            .GetPlayerInfoAsync((PlayerUid)role, token)
            .ConfigureAwait(false);
        Must.NotNull(playerInfo!);

        List<Character> characters = await gameRecordClient
            .GetCharactersAsync((PlayerUid)role, playerInfo, token)
            .ConfigureAwait(false);

        SpiralAbyss? spiralAbyssInfo = await gameRecordClient
            .GetSpiralAbyssAsync((PlayerUid)role, SpiralAbyssSchedule.Current, token)
            .ConfigureAwait(false);
        Must.NotNull(spiralAbyssInfo!);

        return PlayerRecord.Create(role.GameUid, characters, spiralAbyssInfo);
    }

    /// <summary>
    /// 异步获取所有记录并上传到数据库
    /// </summary>
    /// <param name="confirmAsyncFunc">异步确认委托</param>
    /// <param name="resultAsyncFunc">结果确认委托</param>
    /// <param name="token">取消令牌</param>
    /// <returns>任务</returns>
    [Obsolete("上传任务应交由视图模型完成")]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public async Task GetAllRecordsAndUploadAsync(Func<PlayerRecord, Task<bool>> confirmAsyncFunc, Func<Response.Response, Task> resultAsyncFunc, CancellationToken token = default)
    {
        // 由于此方法需要直接与UI线程交互
        // 内部的异步方法均不使用 .ConfigureAwait(false);
        List<UserGameRole> userGameRoles = await userGameRoleClient
            .GetUserGameRolesAsync(token);

        foreach (UserGameRole role in userGameRoles)
        {
            PlayerRecord playerRecord = await GetPlayerRecordAsync(role, token);

            if (await confirmAsyncFunc(playerRecord))
            {
                Response<string>? resp = null;

                if (await playerRecord.UploadItemsAsync(this, token))
                {
                    await playerRecord.UploadRecordAsync(this, token);
                }

                await resultAsyncFunc(resp ?? Response.Response.CreateForException($"{role.GameUid}-记录提交失败。"));
            }
        }
    }

    /// <summary>
    /// 异步上传记录
    /// </summary>
    /// <param name="playerRecord">玩家记录</param>
    /// <param name="token">取消令牌</param>
    /// <returns>响应</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal async Task<Response<string>?> UploadRecordAsync(PlayerRecord playerRecord, CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        HttpResponseMessage response = await httpClient.PostAsJsonAsync($"{HutaoAPI}/Record/Upload", playerRecord, jsonSerializerOptions, token);
        return await response.Content.ReadFromJsonAsync<Response<string>>(jsonSerializerOptions, token);
    }

    /// <summary>
    /// 异步上传物品所有物品
    /// </summary>
    /// <param name="characters">角色详细信息</param>
    /// <param name="token">取消令牌</param>
    /// <returns>响应</returns>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal async Task<Response<string>?> UploadItemsAsync(List<Character> characters, CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        IEnumerable<Item> avatars = characters
            .Select(avatar => new Item(avatar.Id, avatar.Name, avatar.Icon))
            .DistinctBy(item => item.Id);

        IEnumerable<Item> weapons = characters
            .Select(avatar => avatar.Weapon)
            .Select(weapon => new Item(weapon.Id, weapon.Name, weapon.Icon))
            .DistinctBy(item => item.Id);

        IEnumerable<Item> reliquaries = characters
            .Select(avatars => avatars.Reliquaries)
            .Flatten()
            .Where(relic => relic.Position == ReliquaryPosition.FlowerOfLife)
            .DistinctBy(relic => relic.Id)
            .Select(relic => new Item(relic.ReliquarySet.Id, relic.ReliquarySet.Name, relic.Icon));

        GenshinItemWrapper? data = new(avatars, weapons, reliquaries);
        JsonSerializerOptions? option = Ioc.Default.GetService<JsonSerializerOptions>();

        HttpResponseMessage? response = await httpClient
            .PostAsJsonAsync($"{HutaoAPI}​/GenshinItem/Upload", data, jsonSerializerOptions, token)
            .ConfigureAwait(false);

        return await response.Content
            .ReadFromJsonAsync<Response<string>>(jsonSerializerOptions, token)
            .ConfigureAwait(false);
    }

    private class Auth
    {
        public Auth(string appid, string secret)
        {
            Appid = appid;
            Secret = secret;
        }

        public string Appid { get; }

        public string Secret { get; }
    }

    private class Token
    {
        public string AccessToken { get; } = default!;
    }
}
