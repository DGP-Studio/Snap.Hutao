// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

/// <summary>
/// 用户
/// </summary>
[Table("users")]
public class User
{
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
}