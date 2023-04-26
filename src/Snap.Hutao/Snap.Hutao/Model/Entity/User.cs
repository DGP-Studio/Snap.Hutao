// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Web.Hoyolab;
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
    /// 用户的 LToken
    /// </summary>
    [Column("Ltoken")] // DO NOT RENAME
    public Cookie? LToken { get; set; }

    /// <summary>
    /// 用户的 SToken V2
    /// </summary>
    [Column("Stoken")] // DO NOT RENAME
    public Cookie? SToken { get; set; }

    /// <summary>
    /// 是否为国际服账号
    /// </summary>
    public bool IsOversea { get; set; }

    /// <summary>
    /// 创建一个新的用户
    /// </summary>
    /// <param name="cookie">cookie</param>
    /// <param name="isOversea">是否为国际服</param>
    /// <returns>新创建的用户</returns>
    public static User Create(Cookie cookie, bool isOversea)
    {
        _ = cookie.TryGetSToken(isOversea, out Cookie? stoken);
        _ = cookie.TryGetLToken(out Cookie? ltoken);
        _ = cookie.TryGetCookieToken(out Cookie? cookieToken);

        return new() { SToken = stoken, LToken = ltoken, CookieToken = cookieToken };
    }
}