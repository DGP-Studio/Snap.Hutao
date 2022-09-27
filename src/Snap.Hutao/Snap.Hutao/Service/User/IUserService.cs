// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Microsoft.UI.Xaml.Automation.Provider;
using Snap.Hutao.Core.Threading;
using Snap.Hutao.Model.Binding;
using System.Collections.ObjectModel;

namespace Snap.Hutao.Service.Abstraction;

/// <summary>
/// 用户服务
/// </summary>
public interface IUserService
{
    /// <summary>
    /// 获取或设置当前用户
    /// </summary>
    User? CurrentUser { get; set; }

    /// <summary>
    /// 异步获取同步的用户信息集合
    /// 对集合的操作应通过服务抽象完成
    /// 此操作不能取消
    /// </summary>
    /// <returns>准备完成的用户信息枚举</returns>
    Task<ObservableCollection<User>> GetUserCollectionAsync();

    /// <summary>
    /// 异步添加用户
    /// 通常用户是未初始化的
    /// </summary>
    /// <param name="user">待添加的用户</param>
    /// <param name="uid">用户的米游社UID,用于检查是否包含重复的用户</param>
    /// <returns>用户初始化是否成功</returns>
    Task<UserAddResult> TryAddUserAsync(User user, string uid);

    /// <summary>
    /// 尝试使用 login_ticket 升级用户
    /// </summary>
    /// <param name="addiition">额外的Cookie</param>
    /// <param name="token">取消令牌</param>
    /// <returns>是否升级成功</returns>
    Task<ValueResult<bool, string>> TryUpgradeUserByLoginTicketAsync(IDictionary<string, string> addiition, CancellationToken token = default);

    /// <summary>
    /// 尝试使用 Stoken 升级用户
    /// </summary>
    /// <param name="stoken">stoken</param>
    /// <returns>是否升级成功</returns>
    Task<ValueResult<bool, string>> TryUpgradeUserByStokenAsync(IDictionary<string, string> stoken);

    /// <summary>
    /// 异步移除用户
    /// </summary>
    /// <param name="user">待移除的用户</param>
    /// <returns>任务</returns>
    Task RemoveUserAsync(User user);

    /// <summary>
    /// 创建一个新的绑定用户
    /// 若存在 login_ticket 与 login_uid 则 自动获取 stoken
    /// </summary>
    /// <param name="cookie">cookie的字符串形式</param>
    /// <returns>新的绑定用户</returns>
    Task<User?> CreateUserAsync(IDictionary<string, string> cookie);
}
