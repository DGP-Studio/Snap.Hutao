// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Threading;
using Snap.Hutao.Model.Binding.LaunchGame;
using Snap.Hutao.Model.Entity;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Game;

/// <summary>
/// 游戏服务
/// </summary>
internal interface IGameService
{
    /// <summary>
    /// 将账号绑定到对应的Uid
    /// 清除老账号的绑定状态
    /// </summary>
    /// <param name="gameAccount">游戏内账号</param>
    /// <param name="uid">uid</param>
    void AttachGameAccountToUid(GameAccount gameAccount, string uid);

    /// <summary>
    /// 检测并尝试添加游戏内账户
    /// </summary>
    /// <returns>任务</returns>
    ValueTask DetectGameAccountAsync();

    /// <summary>
    /// 获取游戏内账号集合
    /// </summary>
    /// <returns>游戏内账号集合</returns>
    ObservableCollection<GameAccount> GetGameAccountCollection();

    /// <summary>
    /// 异步获取游戏路径
    /// </summary>
    /// <returns>结果</returns>
    ValueTask<ValueResult<bool, string>> GetGamePathAsync();

    /// <summary>
    /// 获取游戏路径，跳过异步定位器
    /// </summary>
    /// <returns>游戏路径，当路径无效时会设置并返回 <see cref="string.Empty"/></returns>
    string GetGamePathSkipLocator();

    /// <summary>
    /// 获取多通道值
    /// </summary>
    /// <returns>多通道值</returns>
    MultiChannel GetMultiChannel();

    /// <summary>
    /// 游戏是否正在运行
    /// </summary>
    /// <returns>是否正在运行</returns>
    bool IsGameRunning();

    /// <summary>
    /// 异步启动
    /// </summary>
    /// <param name="configuration">启动配置</param>
    /// <returns>任务</returns>
    ValueTask LaunchAsync(LaunchConfiguration configuration);

    /// <summary>
    /// 异步修改游戏账号名称
    /// </summary>
    /// <param name="gameAccount">游戏账号</param>
    /// <returns>任务</returns>
    ValueTask ModifyGameAccountAsync(GameAccount gameAccount);

    /// <summary>
    /// 重写游戏路径
    /// </summary>
    /// <param name="path">路径</param>
    void OverwriteGamePath(string path);

    /// <summary>
    /// 异步尝试移除账号
    /// </summary>
    /// <param name="gameAccount">账号</param>
    /// <returns>任务</returns>
    ValueTask RemoveGameAccountAsync(GameAccount gameAccount);

    /// <summary>
    /// 修改注册表中的账号信息
    /// </summary>
    /// <param name="account">账号</param>
    /// <returns>是否设置成功</returns>
    bool SetGameAccount(GameAccount account);

    /// <summary>
    /// 设置多通道值
    /// </summary>
    /// <param name="scheme">方案</param>
    void SetMultiChannel(LaunchScheme scheme);
}