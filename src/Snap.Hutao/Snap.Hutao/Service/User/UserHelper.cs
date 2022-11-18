// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab;
using System.Collections.ObjectModel;
using BindingUser = Snap.Hutao.Model.Binding.User.User;

namespace Snap.Hutao.Service.User;

/// <summary>
/// 用户帮助类
/// </summary>
internal static class UserHelper
{
    /// <summary>
    /// 尝试获取用户
    /// </summary>
    /// <param name="users">待查找的用户集合</param>
    /// <param name="mid">米哈游Id</param>
    /// <param name="user">用户</param>
    /// <returns>是否存在用户</returns>
    public static bool TryGetUser(ObservableCollection<BindingUser> users, string mid, [NotNullWhen(true)] out BindingUser? user)
    {
        user = users.SingleOrDefault(u => u.Entity.Mid == mid);
        return user != null;
    }
}