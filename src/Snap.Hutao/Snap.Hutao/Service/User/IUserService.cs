// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.ViewModel.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using System.Collections.ObjectModel;
using BindingUser = Snap.Hutao.ViewModel.User.User;
using EntityUser = Snap.Hutao.Model.Entity.User;

namespace Snap.Hutao.Service.User;

/// <summary>
/// 用户服务
/// </summary>
[HighQuality]
internal interface IUserService
{
    /// <summary>
    /// 获取或设置当前用户
    /// </summary>
    BindingUser? Current { get; set; }

    /// <summary>
    /// 异步获取角色与用户集合
    /// </summary>
    /// <returns>角色与用户集合</returns>
    ValueTask<ObservableCollection<UserAndUid>> GetRoleCollectionAsync();

    /// <summary>
    /// 初始化用户服务及所有用户
    /// 异步获取同步的用户信息集合
    /// 对集合的操作应通过服务抽象完成
    /// 此操作不能取消
    /// </summary>
    /// <returns>准备完成的用户信息集合</returns>
    ValueTask<ObservableReorderableDbCollection<BindingUser, EntityUser>> GetUserCollectionAsync();

    /// <summary>
    /// 获取角色信息
    /// </summary>
    /// <param name="uid">uid</param>
    /// <returns>对应的角色信息</returns>
    UserGameRole? GetUserGameRoleByUid(string uid);

    ValueTask<ValueResult<UserOptionResult, string>> ProcessInputCookieAsync(InputCookie inputCookie);

    ValueTask<bool> RefreshCookieTokenAsync(Model.Entity.User user);

    /// <summary>
    /// 异步移除用户
    /// </summary>
    /// <param name="user">待移除的用户</param>
    /// <returns>任务</returns>
    ValueTask RemoveUserAsync(BindingUser user);

    ValueTask RefreshUidProfilePictureAsync(UserGameRole userGameRole);
}