// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Entity.Abstraction;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

/// <summary>
/// 消耗物品
/// </summary>
[HighQuality]
[Table("cultivate_items")]
internal sealed class CultivateItem : IDbMappingForeignKeyFrom<CultivateItem, Web.Hoyolab.Takumi.Event.Calculate.Item>
{
    /// <summary>
    /// 内部Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    /// <summary>
    /// 外键
    /// </summary>
    public Guid EntryId { get; set; }

    /// <summary>
    /// 入口名称
    /// </summary>
    [ForeignKey(nameof(EntryId))]
    public CultivateEntry Entry { get; set; } = default!;

    /// <summary>
    /// 物品 Id
    /// </summary>
    public int ItemId { get; set; }

    /// <summary>
    /// 物品个数
    /// </summary>
    public int Count { get; set; }

    /// <summary>
    /// 是否完成此项
    /// </summary>
    public bool IsFinished { get; set; }

    /// <summary>
    /// 创建一个新的养成物品
    /// </summary>
    /// <param name="entryId">入口点 Id</param>
    /// <param name="item">物品</param>
    /// <returns>养成物品</returns>
    public static CultivateItem From(in Guid entryId, in Web.Hoyolab.Takumi.Event.Calculate.Item item)
    {
        return new()
        {
            EntryId = entryId,
            ItemId = item.Id,
            Count = item.Num,
        };
    }
}