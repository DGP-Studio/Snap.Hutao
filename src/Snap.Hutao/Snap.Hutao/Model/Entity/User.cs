// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Bbs.User;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

/// <summary>
/// 用户
/// </summary>
[Table("users")]
public class User
{
    /// <summary>
    /// 用户的Cookie
    /// </summary>
    public string? Cookie { get; set; }

    /// <summary>
    /// 用户信息
    /// </summary>
    [NotMapped]
    public UserInfo? UserInfo { get; set; }
}