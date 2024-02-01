// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity;
using Snap.Hutao.Model.Entity.Primitive;
using Snap.Hutao.Service.Game.Configuration;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Game;

/// <summary>
/// 游戏服务
/// </summary>
[HighQuality]
internal interface IGameServiceFacade
{
    /// <summary>
    /// 游戏内账号集合
    /// </summary>
    ObservableCollection<GameAccount> GameAccountCollection { get; }

    /// <summary>
    /// 将账号绑定到对应的Uid
    /// 清除老账号的绑定状态
    /// </summary>
    /// <param name="gameAccount">游戏内账号</param>
    /// <param name="uid">uid</param>
    void AttachGameAccountToUid(GameAccount gameAccount, string uid);

    ValueTask<GameAccount?> DetectGameAccountAsync(SchemeType scheme);

    /// <summary>
    /// 异步获取游戏路径
    /// </summary>
    /// <returns>结果</returns>
    ValueTask<ValueResult<bool, string>> GetGamePathAsync();

    /// <summary>
    /// 获取多通道值
    /// </summary>
    /// <returns>多通道值</returns>
    ChannelOptions GetChannelOptions();

    /// <summary>
    /// 游戏是否正在运行
    /// </summary>
    /// <returns>是否正在运行</returns>
    bool IsGameRunning();

    /// <summary>
    /// 异步修改游戏账号名称
    /// </summary>
    /// <param name="gameAccount">游戏账号</param>
    /// <returns>任务</returns>
    ValueTask ModifyGameAccountAsync(GameAccount gameAccount);

    /// <summary>
    /// 异步尝试移除账号
    /// </summary>
    /// <param name="gameAccount">账号</param>
    /// <returns>任务</returns>
    ValueTask RemoveGameAccountAsync(GameAccount gameAccount);

    GameAccount? DetectCurrentGameAccount(SchemeType scheme);

    void ReorderGameAccounts(IEnumerable<GameAccount> reorderedGameAccounts);
}