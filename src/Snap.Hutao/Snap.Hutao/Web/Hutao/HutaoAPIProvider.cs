// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Extension;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.SpiralAbyss;
using Snap.Hutao.Web.Hutao.Model;
using Snap.Hutao.Web.Hutao.Model.Post;
using Snap.Hutao.Web.Request;
using Snap.Hutao.Web.Response;
using System.Collections.Generic;
using System.Linq;

namespace Snap.Hutao.Web.Hutao;

/// <summary>
/// 胡桃API提供器
/// </summary>
[Injection(InjectAs.Transient)]
internal class HutaoAPIProvider : IAsyncInitializable
{
    private const string AuthAPIHost = "https://auth.snapgenshin.com";
    private const string HutaoAPI = "https://hutao-api.snapgenshin.com";
    private const string PostContentType = "text/json";

    private readonly Requester requester;
    private readonly AuthRequester authRequester;
    private readonly GameRecordProvider gameRecordProvider;
    private readonly UserGameRoleProvider userGameRoleProvider;

    private bool isInitialized = false;

    /// <summary>
    /// 构造一个新的胡桃API提供器
    /// </summary>
    /// <param name="requester">请求器</param>
    /// <param name="authRequester">支持验证的请求器</param>
    /// <param name="gameRecordProvider">游戏记录提供器</param>
    /// <param name="userGameRoleProvider">用户游戏角色提供器</param>
    public HutaoAPIProvider(
        Requester requester,
        AuthRequester authRequester,
        GameRecordProvider gameRecordProvider,
        UserGameRoleProvider userGameRoleProvider)
    {
        this.requester = requester;
        this.authRequester = authRequester;
        this.gameRecordProvider = gameRecordProvider;
        this.userGameRoleProvider = userGameRoleProvider;
    }

    /// <inheritdoc/>
    public bool IsInitialized { get => isInitialized; }

    /// <inheritdoc/>
    public async Task<bool> InitializeAsync(CancellationToken token = default)
    {
        Auth auth = new(
            "08d9e212-0cb3-4d71-8ed7-003606da7b20",
            "7ueWgZGn53dDhrm8L5ZRw+YWfOeSWtgQmJWquRgaygw=");

        Response<Token>? resp = await requester
            .Reset()
            .PostAsync<Token>($"{AuthAPIHost}/Auth/Login", auth, PostContentType, token)
            .ConfigureAwait(false);

        authRequester.AuthToken = Must.NotNull(resp?.Data?.AccessToken!);
        isInitialized = true;

        return isInitialized;
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

        Response<UploadStatus>? resp = await authRequester
            .GetAsync<UploadStatus>($"{HutaoAPI}/Record/CheckRecord/{uid}", token)
            .ConfigureAwait(false);
        return resp?.Data is not null && resp.Data.PeriodUploaded;
    }

    /// <summary>
    /// 异步获取总览数据
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>总览信息</returns>
    public async Task<Overview?> GetOverviewAsync(CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        Response<Overview>? resp = await authRequester
            .GetAsync<Overview>($"{HutaoAPI}/Statistics/Overview", token)
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

        Response<IEnumerable<AvatarParticipation>>? resp = await authRequester
            .GetAsync<IEnumerable<AvatarParticipation>>($"{HutaoAPI}/Statistics/AvatarParticipation", token)
            .ConfigureAwait(false);

        return resp?.Data ?? Enumerable.Empty<AvatarParticipation>();
    }

    /// <summary>
    /// 异步获取角色圣遗物搭配
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色圣遗物搭配</returns>
    public async Task<IEnumerable<AvatarReliquaryUsage>> GetAvatarReliquaryUsagesAsync(CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        Response<IEnumerable<AvatarReliquaryUsage>>? resp = await authRequester
            .GetAsync<IEnumerable<AvatarReliquaryUsage>>($"{HutaoAPI}/Statistics/AvatarReliquaryUsage", token)
            .ConfigureAwait(false);

        return resp?.Data ?? Enumerable.Empty<AvatarReliquaryUsage>();
    }

    /// <summary>
    /// 异步获取角色搭配数据
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色搭配数据</returns>
    public async Task<IEnumerable<TeamCollocation>> GetTeamCollocationsAsync(CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        Response<IEnumerable<TeamCollocation>>? resp = await authRequester
            .GetAsync<IEnumerable<TeamCollocation>>($"{HutaoAPI}/Statistics/TeamCollocation", token)
            .ConfigureAwait(false);

        return resp?.Data ?? Enumerable.Empty<TeamCollocation>();
    }

    /// <summary>
    /// 异步获取角色武器搭配数据
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色武器搭配数据</returns>
    public async Task<IEnumerable<AvatarWeaponUsage>> GetAvatarWeaponUsagesAsync(CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        Response<IEnumerable<AvatarWeaponUsage>>? resp = await authRequester
            .GetAsync<IEnumerable<AvatarWeaponUsage>>($"{HutaoAPI}/Statistics/AvatarWeaponUsage", token)
            .ConfigureAwait(false);

        return resp?.Data ?? Enumerable.Empty<AvatarWeaponUsage>();
    }

    /// <summary>
    /// 异步获取角色命座信息
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色图片列表</returns>
    public async Task<IEnumerable<AvatarConstellationNum>> GetAvatarConstellationsAsync(CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        Response<IEnumerable<AvatarConstellationNum>>? resp = await authRequester
            .GetAsync<IEnumerable<AvatarConstellationNum>>($"{HutaoAPI}/Statistics/Constellation", token)
            .ConfigureAwait(false);

        return resp?.Data ?? Enumerable.Empty<AvatarConstellationNum>();
    }

    /// <summary>
    /// 异步获取队伍出场次数
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>队伍出场列表</returns>
    public async Task<IEnumerable<TeamCombination>> GetTeamCombinationsAsync(CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        Response<IEnumerable<TeamCombination>>? resp = await authRequester
            .GetAsync<IEnumerable<TeamCombination>>($"{HutaoAPI}/Statistics/TeamCombination", token)
            .ConfigureAwait(false);
        return resp?.Data ?? Enumerable.Empty<TeamCombination>();
    }

    /// <summary>
    /// 异步获取角色图片列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>角色图片列表</returns>
    public async Task<IEnumerable<Item>> GetAvatarMapAsync(CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        Response<IEnumerable<Item>>? resp = await authRequester
            .GetAsync<IEnumerable<Item>>($"{HutaoAPI}/GenshinItem/Avatars", token)
            .ConfigureAwait(false);

        return resp?.Data ?? Enumerable.Empty<Item>();
    }

    /// <summary>
    /// 异步获取武器图片列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>武器图片列表</returns>
    public async Task<IEnumerable<Item>> GetWeaponMapAsync(CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        Response<IEnumerable<Item>>? resp = await authRequester
            .GetAsync<IEnumerable<Item>>($"{HutaoAPI}/GenshinItem/Weapons", token)
            .ConfigureAwait(false);

        return resp?.Data ?? Enumerable.Empty<Item>();
    }

    /// <summary>
    /// 异步获取圣遗物图片列表
    /// </summary>
    /// <param name="token">取消令牌</param>
    /// <returns>圣遗物图片列表</returns>
    public async Task<IEnumerable<Item>> GetReliquaryMapAsync(CancellationToken token = default)
    {
        Verify.Operation(IsInitialized, "必须在初始化后才能调用其他方法");

        Response<IEnumerable<Item>>? resp = await authRequester
            .GetAsync<IEnumerable<Item>>($"{HutaoAPI}/GenshinItem/Reliquaries", token)
            .ConfigureAwait(false);

        return resp?.Data ?? Enumerable.Empty<Item>();
    }

    /// <summary>
    /// 异步获取所有记录并上传到数据库
    /// </summary>
    /// <param name="confirmAsyncFunc">异步确认委托</param>
    /// <param name="resultAsyncFunc">结果确认委托</param>
    /// <param name="token">取消令牌</param>
    /// <returns>任务</returns>
    public async Task GetAllRecordsAndUploadAsync(Func<PlayerRecord, Task<bool>> confirmAsyncFunc, Func<Response.Response, Task> resultAsyncFunc, CancellationToken token = default)
    {
        List<UserGameRole> userGameRoles = await userGameRoleProvider
            .GetUserGameRolesAsync(token);

        foreach (UserGameRole role in userGameRoles)
        {
            PlayerInfo? playerInfo = await gameRecordProvider
                .GetPlayerInfoAsync(role.AsPlayerUid(), token);
            Must.NotNull(playerInfo!);

            List<Character> characters = await gameRecordProvider
                .GetCharactersAsync(role.AsPlayerUid(), playerInfo, token);

            SpiralAbyss? spiralAbyssInfo = await gameRecordProvider
                .GetSpiralAbyssAsync(role.AsPlayerUid(), SpiralAbyssSchedule.Current, token);
            Must.NotNull(spiralAbyssInfo!);

            PlayerRecord playerRecord = PlayerRecord.Create(role.GameUid, characters, spiralAbyssInfo);
            if (await confirmAsyncFunc(playerRecord))
            {
                Response<string>? resp = null;
                if (Response.Response.IsOk(await UploadItemsAsync(characters, token)))
                {
                    resp = await authRequester
                        .PostAsync<string>($"{HutaoAPI}/Record/Upload", playerRecord, PostContentType, token);
                }

                await resultAsyncFunc(resp ?? Response.Response.CreateForException($"{role.GameUid}-记录提交失败。"));
            }
        }
    }

    /// <summary>
    /// 异步上传物品所有物品
    /// </summary>
    /// <param name="characters">角色详细信息</param>
    /// <param name="token">取消令牌</param>
    /// <returns>响应</returns>
    private async Task<Response<string>?> UploadItemsAsync(List<Character> characters, CancellationToken token = default)
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

        return await authRequester
            .PostAsync<string>($"{HutaoAPI}​/GenshinItem/Upload", data, PostContentType, token)
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
