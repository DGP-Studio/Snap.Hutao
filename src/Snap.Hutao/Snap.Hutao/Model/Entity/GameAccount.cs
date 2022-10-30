// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Binding.LaunchGame;
using Snap.Hutao.Web.Hoyolab;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

/// <summary>
/// 游戏内账号
/// </summary>
[Table("game_accounts")]
public class GameAccount : ISelectable
{
    /// <summary>
    /// 内部Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    /// <inheritdoc/>
    public bool IsSelected { get; set; }

    /// <summary>
    /// 对应的Uid
    /// </summary>
    public string? AttachUid { get; set; }

    /// <summary>
    /// 服务器类型
    /// </summary>
    public SchemeType Type { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// MIHOYOSDK_ADL_PROD_CN_h3123967166
    /// </summary>
    public string MihoyoSDK { get; set; } = default!;
}