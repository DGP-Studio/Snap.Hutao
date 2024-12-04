// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database.Abstraction;
using Snap.Hutao.Web.Hoyolab;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

[Table("users")]
internal sealed class User : ISelectable, IReorderable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    public string? Aid { get; set; }

    public string? Mid { get; set; }

    public bool IsSelected { get; set; }

    public Cookie? CookieToken { get; set; }

    [Column("Ltoken")] // DO NOT RENAME
    public Cookie? LToken { get; set; }

    [Column("Stoken")] // DO NOT RENAME
    public Cookie? SToken { get; set; }

    public bool IsOversea { get; set; }

    public string? Fingerprint { get; set; }

    public DateTimeOffset FingerprintLastUpdateTime { get; set; }

    public DateTimeOffset CookieTokenLastUpdateTime { get; set; }

    public int Index { get; set; }

    public string? PreferredUid { get; set; }

    public static User From(Cookie cookie)
    {
        _ = cookie.TryGetSToken(out Cookie? sToken);
        _ = cookie.TryGetLToken(out Cookie? lToken);
        _ = cookie.TryGetCookieToken(out Cookie? cookieToken);

        return new() { SToken = sToken, LToken = lToken, CookieToken = cookieToken };
    }
}