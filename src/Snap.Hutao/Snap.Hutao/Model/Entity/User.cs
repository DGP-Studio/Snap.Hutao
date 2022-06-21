// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Extension;
using Snap.Hutao.Web.Hoyolab.Bbs.User;
using Snap.Hutao.Web.Hoyolab.Takumi.Binding;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Snap.Hutao.Model.Entity;

/// <summary>
/// 用户
/// </summary>
[Table("users")]
public class User : Observable
{
    /// <summary>
    /// 无用户
    /// </summary>
    public static readonly User None = new();
    private UserGameRole? selectedUserGameRole;

    /// <summary>
    /// 内部Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    /// <summary>
    /// 是否被选中
    /// </summary>
    public bool IsSelected { get; set; }

    /// <summary>
    /// 用户的Cookie
    /// </summary>
    public string? Cookie { get; set; }

    /// <summary>
    /// 用户信息
    /// </summary>
    [NotMapped]
    public UserInfo? UserInfo { get; private set; }

    /// <summary>
    /// 用户信息
    /// </summary>
    [NotMapped]
    public List<UserGameRole>? UserGameRoles { get; private set; }

    /// <summary>
    /// 用户信息
    /// </summary>
    [NotMapped]
    public UserGameRole? SelectedUserGameRole
    {
        get => selectedUserGameRole;
        private set => Set(ref selectedUserGameRole, value);
    }

    /// <summary>
    /// 判断用户是否为空用户
    /// </summary>
    /// <param name="user">待检测的用户</param>
    /// <returns>是否为空用户</returns>
    public static bool IsNone([NotNullWhen(false)] User? user)
    {
        return ReferenceEquals(NoneIfNullOrNoCookie(user), None);
    }

    /// <summary>
    /// 设置用户的选中状态
    /// 同时更新用户选择的角色信息
    /// </summary>
    /// <param name="user">用户</param>
    /// <param name="isSelected">是否选中</param>
    public static void SetSelectionState(User user, bool isSelected)
    {
        Verify.Operation(!IsNone(user), "尝试设置一个空的用户");

        user!.IsSelected = isSelected;
        if (isSelected)
        {
            user.SelectedUserGameRole ??= user.UserGameRoles!.FirstOrFirstOrDefault(role => role.IsChosen);
        }
    }

    /// <summary>
    /// 初始化此用户
    /// </summary>
    /// <param name="userClient">用户客户端</param>
    /// <param name="userGameRoleClient">角色客户端</param>
    /// <param name="token">取消令牌</param>
    /// <returns>用户是否初始化完成，若Cookie失效会返回 <see langword="false"/> </returns>
    internal async Task<bool> InitializeAsync(UserClient userClient, UserGameRoleClient userGameRoleClient, CancellationToken token = default)
    {
        if (IsNone(this))
        {
            return false;
        }

        UserInfo = await userClient
            .GetUserFullInfoAsync(this, token)
            .ConfigureAwait(false);

        UserGameRoles = await userGameRoleClient
            .GetUserGameRolesAsync(this, token)
            .ConfigureAwait(false);

        SelectedUserGameRole = UserGameRoles.FirstOrDefault(role => role.IsChosen) ?? UserGameRoles.FirstOrDefault();

        return UserInfo != null && UserGameRoles != null;
    }

    /// <summary>
    /// 尝试尽可能转换为 <see cref="None"/>
    /// </summary>
    /// <param name="user">用户</param>
    /// <returns>转换后的用户</returns>
    private static User NoneIfNullOrNoCookie(User? user)
    {
        if (user is null || user.Cookie == null)
        {
            return None;
        }
        else
        {
            return user;
        }
    }
}