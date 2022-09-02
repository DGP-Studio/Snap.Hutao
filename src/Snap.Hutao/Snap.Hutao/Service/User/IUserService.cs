// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

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
    /// 异步移除用户
    /// </summary>
    /// <param name="user">待移除的用户</param>
    /// <returns>任务</returns>
    Task RemoveUserAsync(User user);

    /// <summary>
    /// 将cookie的字符串形式转换为字典
    /// </summary>
    /// <param name="cookie">cookie的字符串形式</param>
    /// <returns>包含cookie信息的字典</returns>
    IDictionary<string, string> ParseCookie(string cookie);

    /// <summary>
    /// 创建一个新的绑定用户
    /// </summary>
    /// <param name="cookie">cookie的字符串形式</param>
    /// <returns>新的绑定用户</returns>
    Task<User?> CreateUserAsync(string cookie);
}
