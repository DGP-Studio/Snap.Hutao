// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

/// <summary>
/// 抽卡记录物品
/// </summary>
public class GachaItem
{
    /// <summary>
    /// 内部Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    /// <summary>
    /// 存档
    /// </summary>
    public GachaArchive Archive { get; set; } = default!;

    /// <summary>
    /// 存档Id
    /// </summary>
    public Guid ArchiveId { get; set; }

    /// <summary>
    /// 祈愿记录分类
    /// </summary>
    public GachaConfigType GachaType { get; set; }

    /// <summary>
    /// 物品Id
    /// </summary>
    public int ItemId { get; set; }

    /// <summary>
    /// 获取时间
    /// </summary>
    public DateTimeOffset Time { get; set; }

    /// <summary>
    /// 物品
    /// </summary>
    public long Id { get; set; }
}