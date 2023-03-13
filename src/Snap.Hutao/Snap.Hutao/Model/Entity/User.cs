// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Web.Hoyolab;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

/// <summary>
/// 用户
/// </summary>
[HighQuality]
[Table("users")]
internal sealed class User : ISelectable
{
    /// <summary>
    /// 内部Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    /// <summary>
    /// 米游社账号 Id
    /// </summary>
    public string? Aid { get; set; }

    /// <summary>
    /// 米哈游 Id
    /// </summary>
    public string? Mid { get; set; }

    /// <summary>
    /// 是否被选中
    /// </summary>
    public bool IsSelected { get; set; }

    /// <summary>
    /// 用户的 Cookie Token
    /// </summary>
    public Cookie? CookieToken { get; set; }

    /// <summary>
    /// 用户的 Ltoken
    /// </summary>
    public Cookie? Ltoken { get; set; }

    /// <summary>
    /// 用户的 Stoken V2
    /// </summary>
    public Cookie? Stoken { get; set; }

    /// <summary>
    /// 创建一个新的用户
    /// </summary>
    /// <param name="cookie">cookie</param>
    /// <returns>新创建的用户</returns>
    public static User Create(Cookie cookie)
    {
        _ = cookie.TryGetAsStoken(out Cookie? stoken);
        _ = cookie.TryGetAsLtoken(out Cookie? ltoken);
        _ = cookie.TryGetAsCookieToken(out Cookie? cookieToken);

        return new() { Stoken = stoken, Ltoken = ltoken, CookieToken = cookieToken };
    }

    /// <summary>
    /// 创建一个国际服用户
    /// </summary>
    /// <param name="cookie">cookie</param>
    /// <returns>新创建的用户</returns>
    public static User CreateOs(Cookie cookie)
    {
        // 不需要 Stoken
        _ = cookie.TryGetAsLtoken(out Cookie? ltoken);
        _ = cookie.TryGetAsCookieToken(out Cookie? cookieToken);

        return new() { Ltoken = ltoken, CookieToken = cookieToken };
    }
}