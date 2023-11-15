// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity.Abstraction;
using Snap.Hutao.Model.InterChange.GachaLog;
using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace Snap.Hutao.Model.Entity;

/// <summary>
/// 抽卡记录物品
/// </summary>
[HighQuality]
[Table("gacha_items")]
internal sealed partial class GachaItem
    : IDbMappingForeignKeyFrom<GachaItem, GachaLogItem, uint>,
    IDbMappingForeignKeyFrom<GachaItem, UIGFItem, uint>,
    IDbMappingForeignKeyFrom<GachaItem, UIGFItem>,
    IDbMappingForeignKeyFrom<GachaItem, Web.Hutao.GachaLog.GachaItem>
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
    [ForeignKey(nameof(ArchiveId))]
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
    /// 祈愿记录查询分类
    /// 合并保底的卡池使用此属性
    /// 仅4种（不含400）
    /// </summary>
    public GachaConfigType QueryType { get; set; }

    /// <summary>
    /// 物品Id
    /// </summary>
    public uint ItemId { get; set; }

    /// <summary>
    /// 获取时间
    /// </summary>
    public DateTimeOffset Time { get; set; }

    /// <summary>
    /// 物品
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 构造一个新的数据库祈愿物品
    /// </summary>
    /// <param name="archiveId">存档Id</param>
    /// <param name="item">祈愿物品</param>
    /// <param name="itemId">物品Id</param>
    /// <returns>新的祈愿物品</returns>
    public static GachaItem From(in Guid archiveId, in GachaLogItem item, in uint itemId)
    {
        return new()
        {
            ArchiveId = archiveId,
            GachaType = item.GachaType,
            QueryType = item.GachaType.ToQueryType(),
            ItemId = itemId,
            Time = item.Time,
            Id = item.Id,
        };
    }

    /// <summary>
    /// 构造一个新的数据库祈愿物品
    /// </summary>
    /// <param name="archiveId">存档Id</param>
    /// <param name="item">祈愿物品</param>
    /// <param name="itemId">物品Id</param>
    /// <returns>新的祈愿物品</returns>
    public static GachaItem From(in Guid archiveId, in UIGFItem item, in uint itemId)
    {
        return new()
        {
            ArchiveId = archiveId,
            GachaType = item.GachaType,
            QueryType = item.UIGFGachaType,
            ItemId = itemId,
            Time = item.Time,
            Id = item.Id,
        };
    }

    /// <summary>
    /// 构造一个新的数据库祈愿物品
    /// </summary>
    /// <param name="archiveId">存档Id</param>
    /// <param name="item">祈愿物品</param>
    /// <returns>新的祈愿物品</returns>
    public static GachaItem From(in Guid archiveId, in UIGFItem item)
    {
        return new()
        {
            ArchiveId = archiveId,
            GachaType = item.GachaType,
            QueryType = item.UIGFGachaType,
            ItemId = uint.Parse(item.ItemId, CultureInfo.CurrentCulture),
            Time = item.Time,
            Id = item.Id,
        };
    }

    /// <summary>
    /// 构造一个新的数据库祈愿物品
    /// </summary>
    /// <param name="archiveId">存档Id</param>
    /// <param name="item">祈愿物品</param>
    /// <returns>新的祈愿物品</returns>
    public static GachaItem From(in Guid archiveId, in Web.Hutao.GachaLog.GachaItem item)
    {
        return new()
        {
            ArchiveId = archiveId,
            GachaType = item.GachaType,
            QueryType = item.QueryType,
            ItemId = item.ItemId,
            Time = item.Time,
            Id = item.Id,
        };
    }
}