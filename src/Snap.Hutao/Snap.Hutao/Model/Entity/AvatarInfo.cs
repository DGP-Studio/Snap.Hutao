// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

/// <summary>
/// 角色信息表
/// </summary>
[HighQuality]
[Table("avatar_infos")]
internal sealed class AvatarInfo : IMappingFrom<AvatarInfo, string, Web.Enka.Model.AvatarInfo>
{
    /// <summary>
    /// 内部 Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    /// <summary>
    /// Uid
    /// </summary>
    public string Uid { get; set; } = default!;

    /// <summary>
    /// 角色的信息
    /// </summary>
    public Web.Enka.Model.AvatarInfo Info { get; set; } = default!;

    public DateTimeOffset ShowcaseRefreshTime { get; set; }

    public DateTimeOffset GameRecordRefreshTime { get; set; }

    public DateTimeOffset CalculatorRefreshTime { get; set; }

    /// <summary>
    /// 创建一个新的实体角色信息
    /// </summary>
    /// <param name="uid">uid</param>
    /// <param name="info">角色信息</param>
    /// <returns>实体角色信息</returns>
    public static AvatarInfo From(string uid, Web.Enka.Model.AvatarInfo info)
    {
        return new() { Uid = uid, Info = info };
    }
}