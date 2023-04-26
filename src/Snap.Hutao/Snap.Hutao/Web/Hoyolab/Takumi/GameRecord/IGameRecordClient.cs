// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.DependencyInjection.Abstraction;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.Takumi.GameRecord.Avatar;
using Snap.Hutao.Web.Response;

namespace Snap.Hutao.Web.Hoyolab.Takumi.GameRecord;

/// <summary>
/// 游戏记录提供器
/// </summary>
internal interface IGameRecordClient : IOverseaSupport
{
    /// <summary>
    /// 获取玩家角色详细信息
    /// </summary>
    /// <param name="userAndUid">用户与角色</param>
    /// <param name="playerInfo">玩家的基础信息</param>
    /// <param name="token">取消令牌</param>
    /// <returns>角色列表</returns>
    Task<Response<CharacterWrapper>> GetCharactersAsync(UserAndUid userAndUid, PlayerInfo playerInfo, CancellationToken token = default);

    /// <summary>
    /// 异步获取实时便笺
    /// </summary>
    /// <param name="userAndUid">用户与角色</param>
    /// <param name="token">取消令牌</param>
    /// <returns>实时便笺</returns>
    Task<Response<DailyNote.DailyNote>> GetDailyNoteAsync(UserAndUid userAndUid, CancellationToken token = default);

    /// <summary>
    /// 获取玩家基础信息
    /// </summary>
    /// <param name="userAndUid">用户与角色</param>
    /// <param name="token">取消令牌</param>
    /// <returns>玩家的基础信息</returns>
    Task<Response<PlayerInfo>> GetPlayerInfoAsync(UserAndUid userAndUid, CancellationToken token = default);

    /// <summary>
    /// 获取玩家深渊信息
    /// </summary>
    /// <param name="userAndUid">用户</param>
    /// <param name="schedule">1：当期，2：上期</param>
    /// <param name="token">取消令牌</param>
    /// <returns>深渊信息</returns>
    Task<Response<SpiralAbyss.SpiralAbyss>> GetSpiralAbyssAsync(UserAndUid userAndUid, SpiralAbyssSchedule schedule, CancellationToken token = default);
}