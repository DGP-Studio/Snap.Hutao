// Copyright (c) DGP Studio. All rights reserved.
// Licensed under the MIT license.

using Snap.Hutao.Core.Abstraction;
using Snap.Hutao.Core.Database;
using Snap.Hutao.Model.Entity.Abstraction;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Snap.Hutao.Model.Entity;

/// <summary>
/// 培养计划
/// </summary>
[HighQuality]
[Table("cultivate_projects")]
internal sealed class CultivateProject : ISelectable, IMappingFrom<CultivateProject, string, string>, IAppDbEntity
{
    /// <summary>
    /// 内部Id
    /// </summary>
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public Guid InnerId { get; set; }

    /// <summary>
    /// 是否选中
    /// </summary>
    public bool IsSelected { get; set; }

    /// <summary>
    /// 名称
    /// </summary>
    public string Name { get; set; } = default!;

    /// <summary>
    /// 所属的Uid
    /// </summary>
    public string? AttachedUid { get; set; }

    /// <summary>
    /// 创建新的养成计划
    /// </summary>
    /// <param name="name">名称</param>
    /// <param name="attachedUid">绑定的Uid</param>
    /// <returns>新的养成计划</returns>
    public static CultivateProject From(string name, string? attachedUid = null)
    {
        return new() { Name = name, AttachedUid = attachedUid };
    }
}