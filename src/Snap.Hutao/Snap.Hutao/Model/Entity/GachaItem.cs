// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Web.Hoyolab.Hk4e.Event.GachaInfo;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

/// <summary>
/// 抽卡记录物品
/// </summary>
[Table("gacha_items")]
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
    /// 祈愿记录查询分类
    /// 合并保底的卡池使用此属性
    /// 仅4种（不含400）
    /// </summary>
    public GachaConfigType QueryType { get; set; }

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

    /// <summary>
    /// 构造一个新的数据库祈愿物品
    /// </summary>
    /// <param name="archiveId">存档Id</param>
    /// <param name="item">祈愿物品</param>
    /// <param name="itemId">物品Id</param>
    /// <returns>新的祈愿物品</returns>
    public static GachaItem Create(Guid archiveId, GachaLogItem item, int itemId)
    {
        return new()
        {
            ArchiveId = archiveId,
            GachaType = item.GachaType,
            QueryType = ToQueryType(item.GachaType),
            ItemId = itemId,
            Time = item.Time,
            Id = item.Id,
        };
    }

    /// <summary>
    /// 将祈愿配置类型转换到祈愿查询类型
    /// </summary>
    /// <param name="configType">配置类型</param>
    /// <returns>祈愿查询类型</returns>
    public static GachaConfigType ToQueryType(GachaConfigType configType)
    {
        return configType switch
        {
            GachaConfigType.AvatarEventWish2 => GachaConfigType.AvatarEventWish,
            _ => configType,
        };
    }
}