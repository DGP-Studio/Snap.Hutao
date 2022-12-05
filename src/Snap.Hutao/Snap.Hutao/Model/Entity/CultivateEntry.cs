// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Model.Binding.Cultivation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

/// <summary>
/// 养成入口
/// </summary>
[Table("cultivate_entries")]
public class CultivateEntry
{
    /// <summary>
    /// 内部Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    /// <summary>
    /// 项目Id
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// 项目
    /// </summary>
    [ForeignKey(nameof(ProjectId))]
    public CultivateProject Project { get; set; } = default!;

    /// <summary>
    /// 养成类型
    /// </summary>
    public CultivateType Type { get; set; }

    /// <summary>
    /// 角色/武器/家具 Id
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 物品
    /// </summary>
    public virtual ICollection<CultivateItem> Items { get; set; } = default!;
}